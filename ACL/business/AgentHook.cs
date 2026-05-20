using ACL.business.agent;
using ACL.business.log;
using ACL.business.mcp.local;
using ACL.flow;
using OpenAI.Chat;
using System.Text;
using System.Threading.Channels;
using System.Xml.Linq;


namespace ACL.business
{
    class AgentHook
    {

        private static StateMachine state;
        static AgentHook()
        {
            state = new StateMachine();
            TodoStore.Instance.Completed += (s) => state.Push(StateTag.TodoComplete);
            TodoStore.Instance.Inprogressed += (s) => state.Push(StateTag.TodoInprogress);
            TodoStore.Instance.OnCreated += (s) => state.Push(StateTag.TodoCreate);
        }

        public static void Initialize(SessionAgent agent)
        {
            agent.OnBeforeAsk += OnBeforeAsk;
            agent.OnFnCalling += OnFnCalling;
            agent.OnAsyncFnCalling += OnAsyncFnCalling;
            agent.OnAsyncFnCalled += OnAsyncFnCalled;
            agent.OnAsyncFnCalledSuccess += OnAsyncFnCalledSuccess;
            agent.OnAsyncFnCalledError += OnAsyncFnCalledError;
            agent.OnOutput += OnOutput;
            agent.OnLLMException += OnLLMExceptionFound;
        }



        public static async void OnBeforeAsk(HookEventArgs e)
        {
            var channel = e.Input;
            if (channel == null) return;

            var (tag, prompt) = state.Next();

            if (tag == null) return;
            if (prompt == null) return;
            await channel.Writer.WriteAsync(prompt);
        }

        public static void OnFnCalling(HookEventArgs e)
        {
            state.Push(StateTag.FnCall);

            var fnName = e.FnName;
            var parameters = e.FnParameters;
            GlobalLogger.Debug($"[Tool Call] {fnName} {parameters}");
        }

        public static void OnAsyncFnCalling(HookEventArgs e)
        {
        }

        public static void OnAsyncFnCalled(HookEventArgs e)
        {
            GlobalLogger.Debug($"[Tool Result] {e.FnResult}");
        }

        public static async void OnAsyncFnCalledSuccess(HookEventArgs e)
        {
            var channel = e.Input;
            if (channel == null) return;

            var token = e.Token;
            if (token.IsCancellationRequested) return;

            var fnName = e.FnName;
            var result = e.FnResult;

            if (await channel.Writer.WaitToWriteAsync(token))
            {
                await channel.Writer.WriteAsync($"工具{fnName}调用完成，结果为：{result}。请检查一下这个工具{fnName}的输出结果是否存在问题，若存在，请修复。");
            }
        }

        public static async void OnAsyncFnCalledError(HookEventArgs e)
        {
            var channel = e.Input;
            if (channel == null) return;

            var token = e.Token;
            if (token.IsCancellationRequested) return;

            var fnName = e.FnName;
            var error = e.FnError;

            if (await channel.Writer.WaitToWriteAsync(token))
            {
                await channel.Writer.WriteAsync($"工具{fnName}调用存在问题{error}，请执行改正/改进");
            }
        }

        public static void OnOutput(HookEventArgs e)
        {
            var text = e.Text;
            if (text == null) return;

            if (text.Contains("ALL_MISIION_FINISHED"))
            {
                state.Push(StateTag.MissionComplete);
            }
            else
            {
                if (state.Peek() != StateTag.Output) state.Push(StateTag.Output);
            }
        }

        private async static void OnLLMExceptionFound(HookEventArgs e)
        {
            var error = e.Error;
            if (error != null) GlobalLogger.Error(error.Message);

            var messages = e.Messages;
            if (messages == null) return;

            var chatClient = e.ChatClient;
            if (chatClient == null) return;

            //组织所有的非系统的报文
            var sbd = new StringBuilder();

            foreach (var message in messages)
            {
                if (message is SystemChatMessage) continue;

                else if (message is AssistantChatMessage assist)
                {
                    sbd.AppendFormat("[A]{0}\n", assist.Content);
                }

                else if (message is UserChatMessage user)
                {
                    sbd.AppendFormat("[U]{0}\n", user.Content);
                }
                else if (message is ToolChatMessage tool)
                {
                    sbd.AppendFormat("[T]{0}\n", tool.Content);
                }
            }

            var comressedList = new List<ChatMessage>() { new SystemChatMessage(@"
你是一名文案摘要师，负责将所有内容在不损失主要信息的情况下将其压缩到极致。
被压缩后的内容至少包含以下元素：目标，背景，问题，解决步骤，遗留问题，重点记忆。
发送给你的内容中前缀约束如下：
`[U]`代表用户内容
`[A]`代表辅助内容
`[T]`代表工具内容
"), new UserChatMessage($"将以下内容执行压缩:{sbd}") };

            //开始执行压缩
            var chatOptions = new ChatCompletionOptions();
            var token = e.Token;
            var compressed = new StringBuilder();
            var streamingResult = chatClient?.CompleteChatStreamingAsync(comressedList, chatOptions, token);
            if (streamingResult == null)
            {
                GlobalLogger.Error("初始化尚未完成.");
                return;
            }

            await foreach (var update in streamingResult)
            {
                if (update.ContentUpdate.Count > 0)
                {
                    var text = update.ContentUpdate[0].Text;
                    compressed.Append(text);
                }
            }


            messages.RemoveAll(x => !(x is SystemChatMessage));
            //将压缩后的内容替换进去
            messages.Add(new AssistantChatMessage(compressed.ToString()));
        }
    }

}


