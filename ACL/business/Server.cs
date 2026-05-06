using ABL.Object;
using ACL.business;
using ACL.business.llm;
using ACL.business.log;
using ACL.business.mcp;
using ACL.business.project;
using ACL.business.prompt;
using ACL.dao;
using OpenAI.Chat;
using OpenAI.Responses;
using System.ClientModel;
using System.Reflection;
using System.Text;
using System.Threading.Channels;

namespace McpOrchestrator
{
    public class LlamaServer
    {
        static Channel<string> channel = Channel.CreateBounded<string>(10);
        static Channel<string> output = null;
        static CancellationTokenSource chatCts = new CancellationTokenSource(10);
        static ChatClient? chatClient = null;
        static ChatCompletionOptions chatOptions;
        static List<ChatMessage> messages;
        static RelexJSON relexJson;


        static LlamaServer()
        {
            relexJson = new RelexJSON();
            Context.Instance.CurrentSessionChagned += OnCurrentSessionChanged;
            Context.Instance.CurrentLlmModelInfoChagned += OnCurrentLlmModelInfoChagned;
            Context.Instance.CurrentPromptUsageChanged += OnCurrentPromptUsageChanged;
            Context.Instance.McpToolChanged += OnMcpToolsChanged;
        }


        private static void OnCurrentPromptUsageChanged(string usage)
        {
            if (messages == null)
            {
                messages = new List<ChatMessage>();
            }

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

            var prompts = Context.Instance.Prompts;
            foreach (var item in prompts)
            {
                if (item.Type != PromptType.System) continue;

                var prompt = item.Content;
                if (prompt == null || prompt.Length == 0) continue;

                var systemPrompt = prompt.Replace("${project.path}", ProjectConfig.Current.Directory);
                messages.Add(new SystemChatMessage(systemPrompt));
            }
        }

        public static async Task Serve(Channel<string> channel)
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

        public static Task<bool> Chat(string data)
        {
            return Task.FromResult(channel.Writer.TryWrite(data));
        }

        public static async Task RunOrchestrator()
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
                var task = ParseTask(cacheTask);
                if (task != null)
                {
                    Context.Instance.RefreshTask(task);
                    if (!string.IsNullOrEmpty(task.NextActionPlan))
                    {
                        await Task.Run(async () =>
                        {
                            channel?.Writer.WriteAsync(task.NextActionPlan);
                        });
                    }
                }

                var userInput = await channel.Reader.ReadAsync();
                GlobalLogger.Debug($"Received question: {userInput}");

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
                            var fnParams = new StringBuilder();
                            using var fnArgs = new MemoryStream();
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
                                        fnParams.Append(toolCall.FunctionArgumentsUpdate);
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
                                var args = fnParams.ToString();
                                var parameters = BinaryData.FromBytes(fnArgs.ToArray());
                                var toolResult = await Context.Instance.CallToolAsync(fnName, parameters);
                                messages.Add(new AssistantChatMessage(new List<ChatToolCall> { ChatToolCall.CreateFunctionToolCall(fnName, fnName, parameters) }));
                                GlobalLogger.Debug($"[Tool Result] {toolResult.Content}");
                                if (toolResult.Success)
                                {
                                    //messages.Add(new SystemChatMessage($"Tool {fnName} returned: {toolResult.Content}"));
                                    messages.Add(new ToolChatMessage(fnName, $"{toolResult.Content}"));
                                    messages.Add(new UserChatMessage("请检查一下这个工具的输出结果是否存在问题，若存在，请调整调用工具或参数，重新调用获取结果，如果不存在，请按照此次函数调用结果返回所需输出。"));
                                }
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
                        GlobalLogger.Error(e.Message);
                    }
                }
            }
        }

        public static void Cancel()
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



        private static void OnMcpToolsChanged(MCPToolChangedEventArgs e)
        {
            var tools = e.Tools;
            if (tools == null) return;

            var data = CreateMcpTools(tools).Result;
            if (data)
            {

            }
        }

        private async static ValueTask CreateMcpTools()
        {
            var tools = Context.Instance.MCPTools;
            if (tools == null) return;

            await CreateMcpTools(tools);
        }


        private async static Task<bool> CreateMcpTools(List<MCPTool> MCPTools)
        {
            // 模拟 MCP 会话

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

        private static void OnCurrentLlmModelInfoChagned(CurrentLlmModelInfoChangedEventArgs e)
        {
            var modelInfo = e.Current;
            var model = modelInfo.Name;

            var options = new OpenAI.OpenAIClientOptions() { Endpoint = new Uri(modelInfo.AccessUrl) };
            chatClient = new ChatClient(model, new ApiKeyCredential(modelInfo.ApiKey), options);
        }

        static void OnCurrentSessionChanged(SessionChangedEventArgs e)
        {
            var session = e.Current;
            var items = e.SessionItems;
            //获取提示词
            var prompts = Context.Instance.Prompts;

            //系统会话设定
            messages = new List<ChatMessage>();
            foreach (var item in prompts)
            {
                if (item.Type != PromptType.System) continue;

                var prompt = item.Content;
                if (prompt == null || prompt.Length == 0) continue;

                var systemPrompt = prompt.Replace("${project.path}", ProjectConfig.Current.Directory);
                messages.Add(new SystemChatMessage(systemPrompt));
            }

            if (session != null)
            {
                messages.Add(new UserChatMessage(session.Description));
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
        }


        static TaskInfo? ParseTask(StringBuilder cacheTask)
        {
            try
            {
                var cacheLen = cacheTask.Length;
                if (cacheLen > 0)
                {
                    var str = cacheTask.ToString().TrimEnd();
                    var len = str.Length;
                    if (str[0] == '`' && str[1] == '`' && str[2] == '`' && str[3] == 'j' && str[4] == 's' && str[5] == 'o' && str[6] == 'n'
                        && str[len - 1] == '`' && str[len - 2] == '`' && str[len - 3] == '`'
                        )
                    {
                        var json = str.Substring(7, len - 10);
                        var tasks = relexJson.Deserealize<TaskInfo>(json);
                        if (tasks != null && tasks.Count > 0) return tasks[0];
                    }
                }

                return null;
            }
            finally
            {
                cacheTask.Clear();
            }
        }
    }

}