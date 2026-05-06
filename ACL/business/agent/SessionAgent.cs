using ABL.Object;
using ACL.business;
using ACL.business.agent;
using ACL.business.content;
using ACL.business.llm;
using ACL.business.log;
using ACL.business.mcp;
using ACL.business.project;
using ACL.business.session;
using ACL.dao;
using Newtonsoft.Json;
using OpenAI.Chat;
using System.ClientModel;
using System.Configuration;
using System.Text;
using System.Threading.Channels;

namespace ACL.flow
{
    public class ChatMessageList : List<ChatMessage>
    {
        public ChatMessageList()
        {
        }

        public void Add(ChatMessage msg)
        {
            lock (this)
            {
                base.Add(msg);
            }
        }
    }

    public class SessionAgent : IAgent
    {
        private Channel<string>? channel = Channel.CreateBounded<string>(10);
        private Channel<string> output = null;
        private CancellationTokenSource chatCts = new CancellationTokenSource(10);
        private ChatClient? chatClient = null;
        private ChatCompletionOptions chatOptions;
        private ChatMessageList messages;
        private AgentBody? agent;
        private SessionStep step;

        public SessionAgent(AgentBody agent)
        {
            this.agent = agent;
            messages = new ChatMessageList();

            Context.Instance.CurrentLlmModelInfoChagned += OnCurrentLlmModelInfoChagned;
            Context.Instance.AgentRefreshed += OnAgentInfoRefreshed;
            Context.Instance.CurrentSessionChagned += OnCurrentSessionChanged;

            OnCurrentLlmModelInfoChagned(new CurrentLlmModelInfoChangedEventArgs { Current = Context.Instance.CurrentModel });
            OnPromptChanged();
            OnMcpToolsChanged();
            OnCurrentSessionChanged(new SessionChangedEventArgs { Current = Context.Instance.CurrentSession, SessionItems = GetSessionItems() });
        }

        private List<SessionItem> GetSessionItems()
        {
            var id = Context.Instance.CurrentSession.Id;
            var store = new DataStore();
            return store.QuerySessionItems(id);
        }

        public void Dispose()
        {
            Context.Instance.CurrentLlmModelInfoChagned -= OnCurrentLlmModelInfoChagned;
            Context.Instance.AgentRefreshed -= OnAgentInfoRefreshed;
            Context.Instance.CurrentSessionChagned -= OnCurrentSessionChanged;

            channel = null;
            output = null;
            chatCts.Dispose();
            chatClient = null;
            messages.Clear();
            agent = null;
        }

        private void OnAgentInfoRefreshed(AgentChangedEventArgs e)
        {
            OnPromptChanged();
            OnMcpToolsChanged();
        }

        private List<string> GetPrompts()
        {
            var prompts = new List<string>();
            if (!string.IsNullOrEmpty(agent.Defination))
            {
                prompts.Add(agent.Defination);
            }

            if (agent.Skills != null && agent.Skills.Count > 0)
            {
                foreach (var skill in agent.Skills)
                {
                    prompts.Add(skill.SkillPrompt);
                }
            }

            return prompts;
        }

        public void OnPromptChanged()
        {
            for (int i = 0; i < messages.Count; i++)
            {
                var message = messages[i];
                if (message.GetType() != typeof(SystemChatMessage)) break;
                else
                {
                    messages.Remove(message);
                    i--;
                }
            }

            var prompts = GetPrompts();
            foreach (var item in prompts)
            {

                var prompt = item;
                if (prompt == null || prompt.Length == 0) continue;

                var systemPrompt = prompt.Replace("${project.path}", ProjectConfig.Current.Directory);
                messages.Add(new SystemChatMessage(systemPrompt));
            }
        }

        public async Task SetOutput(Channel<string> channel)
        {
            this.output = channel;
        }

        public async Task Serve(Channel<string> channel)
        {
            try
            {
                output = channel;
                await RunOrchestrator();
            }
            catch (Exception ex)
            {
                GlobalLogger.Fatal($"[FATAL] {ex.Message}");
            }
        }

        public Task<bool> Chat(string data)
        {
            return Task.FromResult(channel.Writer.TryWrite(data));
        }

        private async Task<bool> ParseTasks(StringBuilder cacheTask)
        {
            var contentReslover = new ContentReslover(agent?.ContentStores);
            messages.Add(new AssistantChatMessage(cacheTask.ToString()));
            var tasks = contentReslover.Perform(cacheTask.ToString());
            cacheTask.Clear();
            if (tasks != null && tasks.Count > 0)
            {
                foreach (var task in tasks)
                {
                    if (task != null)
                    {
                        Context.Instance.RefreshTask(task);
                        var needHumanToJoin = false;
                        if (!string.IsNullOrEmpty(task.NextActionPlan))
                        {
                            var taskId = task.Id;
                            if (task.SubTaskList != null)
                            {
                                foreach (var subTask in task.SubTaskList)
                                {
                                    if (subTask.NeedHumanJoinStrategy)
                                    {
                                        needHumanToJoin = true;
                                        break;
                                    }
                                }
                            }

                            if (needHumanToJoin)
                            {
                                if (output != null)
                                {
                                    await output.Writer.WriteAsync("请您参与，等待您的决策!\n");
                                    return true;
                                }
                            }
                            else
                            {
                                var test = string.Empty;

                                if (channel != null)
                                {
                                    if (channel.Reader.CanPeek)
                                    {
                                        await Task.Run(async () =>
                                        {
                                            channel?.Writer.WriteAsync(task.NextActionPlan);
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public async Task RunOrchestrator()
        {
            // 获取工具列表
            GlobalLogger.Debug("[*] Fetching tools...");
            await CreateMcpTools();

            // 5. 交互循环
            GlobalLogger.Debug("Enter your question: ");

            //消息缓存
            var cacheTask = new StringBuilder();
            //开始会话
            while (true)
            {
                await ParseTasks(cacheTask);

                string userInput = string.Empty;
                if (channel != null)
                {
                    userInput = await channel.Reader.ReadAsync();
                    GlobalLogger.Debug($"Received question: {userInput}");
                }
                else
                {
                    return;
                }

                // 维护对话历史
                messages.Add(new UserChatMessage(userInput));

                while (true)
                {
                    chatCts = new CancellationTokenSource(60000000);
                    try
                    {
                        using (chatCts)
                        {
                            // 使用流式输出（打字机效果）
                            var streamingResult = chatClient?.CompleteChatStreamingAsync(messages, chatOptions, chatCts.Token);
                            if (streamingResult == null)
                            {
                                GlobalLogger.Error("初始化尚未完成.");
                                break;
                            }

                            string? fnName = null;
                            //var fnParams = new StringBuilder();
                            using var fnArgs = new MemoryStream();
                            //await foreach (var update in streamingResult.ConfigureAwait(false))
                            await foreach (var update in streamingResult)
                            {
                                //调用工具
                                if (update.ToolCallUpdates.Count > 0)
                                {
                                    var toolCall = update.ToolCallUpdates[0];
                                    if (toolCall.FunctionName != null)
                                    {
                                        fnName = toolCall.FunctionName;
                                    }

                                    if (toolCall.FunctionArgumentsUpdate != null)
                                    {
                                        var bytes = toolCall.FunctionArgumentsUpdate.ToArray();
                                        fnArgs.Write(bytes, 0, bytes.Length);
                                        fnArgs.Flush();
                                        //fnParams.Append(toolCall.FunctionArgumentsUpdate);
                                    }
                                }

                                //直接输出了
                                if (update.ContentUpdate.Count > 0)
                                {
                                    var text = update.ContentUpdate[0].Text;
                                    cacheTask.Append(text);
                                    output?.Writer.TryWrite(text);
                                }
                            }

                            if (fnName != null)
                            {
                                //var args = fnParams.ToString();
                                var parameters = BinaryData.FromBytes(fnArgs.ToArray());
                                GlobalLogger.Debug($"[Tool Call] {fnName} {parameters.ToString()}");


                                messages.Add(new AssistantChatMessage(new List<ChatToolCall> { ChatToolCall.CreateFunctionToolCall(fnName, fnName, parameters) }));
                                messages.Add(new ToolChatMessage(fnName, $"工具{fnName}正在调用中，稍后会给你最终调用结果，你先继续。"));

                                new Thread(async () => {
                                    await Task.Run(async () =>
                                    {
                                        var toolResult = await Context.Instance.CallToolAsync(fnName, parameters);
                                        //messages.Add(new AssistantChatMessage(new List<ChatToolCall> { ChatToolCall.CreateFunctionToolCall(fnName, fnName, parameters) }));
                                        GlobalLogger.Debug($"[Tool Result] {toolResult.Content}");
                                        if (toolResult.Success)
                                        {
                                            messages.Add(new ToolChatMessage(fnName, $"工具{fnName}调用完成，结果为：{toolResult.Content}"));
                                            messages.Add(new UserChatMessage($"请检查一下这个工具{fnName}的输出结果是否存在问题，若存在，请调整调用工具或参数，重新调用获取结果，如果不存在，请按照此次函数调用结果返回所需输出。"));
                                        }
                                    });
                                }).Start();

                          


                                continue;
                            }

                            output?.Writer.TryWrite(Environment.NewLine);
                            Console.WriteLine();
                            break;
                        }
                    }
                    catch (OperationCanceledException ex)
                    {
                        GlobalLogger.Error(ex.Message);
                    }
                    catch (Exception e)
                    {
                        if(e.Message .StartsWith("Service request failed.\\r\\nStatus: 400 (Bad Request)"))
                        {
                            var modelInfo = Context.Instance.CurrentModel;
                            var model = modelInfo.Name;
                            var options = new OpenAI.OpenAIClientOptions() { Endpoint = new Uri(modelInfo.AccessUrl) };
                            chatClient = new ChatClient(model, new ApiKeyCredential(modelInfo.ApiKey), options);
                        }

                        GlobalLogger.Error(e.Message);
                    }
                }
            }
        }

        public void Cancel()
        {
            try
            {
                chatCts.Cancel(true);
            }
            catch (Exception e)
            {
                GlobalLogger.Debug(e.GetType().Name + ":" + e.Message);
            }
        }

        public async void OnMcpToolsChanged()
        {
            await CreateMcpTools();
        }

        private async ValueTask CreateMcpTools()
        {
            if (agent?.Tools == null) return;
            var tools = new List<MCPTool>();
            foreach (var tool in agent?.Tools)
            {
                var mcpTool = new MCPTool
                {
                    Name = tool.Name,
                    Description = tool.Description,
                    InputSchema = BinaryData.FromString(tool.InputSchema),
                    OutputSchema = BinaryData.FromString(tool.OutputSchema)
                };

                tools.Add(mcpTool);
            }

            await CreateMcpTools(tools);
        }

        private async Task<bool> CreateMcpTools(List<MCPTool> MCPTools)
        {
            chatOptions = new ChatCompletionOptions();
            if (MCPTools != null)
            {
                foreach (var t in MCPTools)
                {
                    // 使用你提供的 CreateFunctionTool 签名
                    // 将 JSON schema 字符串转为 BinaryData
                    chatOptions.Tools.Add(ChatTool.CreateFunctionTool(
                        t.Name,
                        t.Description,
                        t.InputSchema
                    ));
                }

                GlobalLogger.Debug($"[*] Discovered {MCPTools.Count} tools.");
            }

            return true;
        }

        private void OnCurrentLlmModelInfoChagned(CurrentLlmModelInfoChangedEventArgs e)
        {
            var modelInfo = e.Current;
            var model = modelInfo.Name;
            var options = new OpenAI.OpenAIClientOptions() { Endpoint = new Uri(modelInfo.AccessUrl) };
            chatClient = new ChatClient(model, new ApiKeyCredential(modelInfo.ApiKey), options);
        }

        public void OnCurrentSessionChanged(SessionChangedEventArgs e)
        {
            if (e == null) return;

            messages = new ChatMessageList();

            var session = e.Current;
            var items = e.SessionItems;
            //获取提示词
            var prompts = GetPrompts();

            //系统会话设定
            foreach (var item in prompts)
            {
                var prompt = item;
                if (prompt == null || prompt.Length == 0) continue;

                var systemPrompt = prompt.Replace("${project.path}", ProjectConfig.Current.Directory);
                messages.Add(new SystemChatMessage(systemPrompt));
            }

            if (session != null)
            {
                messages.Add(new UserChatMessage(session.Description));
                messages.Add(new UserChatMessage("你可以通过阅读项目下的文件了解本项目的相关信息。"));
            }

            if (items != null && items.Count > 0)
            {
                foreach (var item in items)
                {
                    switch (item.SessionType)
                    {
                        case SessionType.Sysetem:
                            messages.Add(new SystemChatMessage(item.Description));
                            break;
                        case SessionType.Assistant:
                            messages.Add(new AssistantChatMessage(item.Description));
                            break;
                        case SessionType.User:
                            messages.Add(new UserChatMessage(item.Description));
                            break;
                        case SessionType.Human:
                            messages.Add(new UserChatMessage(item.Description));
                            break;
                        case SessionType.FunctionCall:
                            try
                            {
                                messages.Add(new ToolChatMessage(item.Description));
                            }
                            catch { }
                            break;
                    }
                }
            }

            if (step != null) step.Stop();
            step = new SessionStep(e.Current, messages);
            step.Perform();
        }

    }

}