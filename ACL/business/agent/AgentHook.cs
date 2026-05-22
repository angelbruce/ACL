using ACL.business.log;
using ACL.business.mcp.local;
using ACL.business.session;
using ACL.dao;
using ACL.flow;
using OpenAI.Chat;
using System.Text;


namespace ACL.business.agent
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
            agent.OnBeforeAsking += OnBeforeAsking;
            agent.OnBeforeAsked += OnBeforeAsked;
            agent.OnFnCalling += OnFnCalling;
            agent.OnAsyncFnCalling += OnAsyncFnCalling;
            agent.OnAsyncFnCalled += OnAsyncFnCalled;
            agent.OnAsyncFnCalledSuccess += OnAsyncFnCalledSuccess;
            agent.OnAsyncFnCalledError += OnAsyncFnCalledError;
            agent.OnOutput += OnOutput;
            agent.OnCompressed += OnSessoinCompress;
        }


        public static void PushState(StateTag tag)
        {
            state.Push(tag);
        }


        public static async void OnBeforeAsking(HookEventArgs e)
        {
            var channel = e.Input;
            if (channel == null) return;

            var (tag, prompt) = state.Next();

            if (tag == null) return;
            if (prompt == null) return;
            await channel.Writer.WriteAsync(prompt);
        }


        public static async void OnBeforeAsked(HookEventArgs e)
        {
            var text = e.Text;
            if (string.IsNullOrEmpty(text)) return;
            if (!text.Equals("<compressed />")) return;

            OnSessoinCompress(e);
            e.Cancel = true;
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
            //var channel = e.Input;
            //if (channel == null) return;

            //var token = e.Token;
            //if (token.IsCancellationRequested) return;

            //var fnName = e.FnName;
            //var result = e.FnResult;

            //if (await channel.Writer.WaitToWriteAsync(token))
            //{
            //    await channel.Writer.WriteAsync($"请检查一下工具{fnName}结果{result}是否有问题，若存在，请修复；否则，请继续。");
            //}
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
                await channel.Writer.WriteAsync($"工具{fnName}调用存在问题{error}，请立刻执行改正/改进");
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

        private async static void OnSessoinCompress(HookEventArgs e)
        {
            var error = e.Error;
            if (error != null) GlobalLogger.Error(error.Message);
            if (e.Error?.Message != null && e.Error.Message.Contains("HTTP 500")) return;

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
                    sbd.AppendFormat("[A]{0}\n", GetText(assist));
                }

                else if (message is UserChatMessage user)
                {
                    sbd.AppendFormat("[U]{0}\n", GetText(user));
                }
                else if (message is ToolChatMessage tool)
                {
                    sbd.AppendFormat("[T]{0}\n", GetText(tool));
                }
            }

            var comressedList = new List<ChatMessage>() { new SystemChatMessage(@"
你是一名精准的文案摘要师，负责将所有内容在不损失主要信息的情况下将其压缩到极致。
被压缩后的内容至少包含以下元素：目标，背景，问题，解决步骤，任务完成情况，遗留问题，重点记忆。
要求摘要后的重点不能丢失。
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

            var session = Context.Instance.CurrentSession;
            if (session == null) return;

            var store = new DataStore();
            store.DeleteSessionItemsBySessionId(session.Id);

            var item = new SessionItem()
            {
                Id = DateTime.Now.Ticks,
                Description = compressed.ToString(),
                SessionId = session.Id,
                SessionType = SessionType.Assistant,
                State = ABL.Object.EnumEntityState.Added
            };

            store.Save(item);

            Instance<PostOffice>.Data?.Post(business.session.Message.AICompressed);
        }

        static string GetText(ChatMessage chat)
        {
            var content = chat.Content;
            var sbd = new StringBuilder();
            foreach (var part in content)
            {
                switch (part.Kind)
                {
                    case ChatMessageContentPartKind.Text:
                        sbd.Append(part.Text);
                        break;
                    case ChatMessageContentPartKind.Image:
                        break;
                    case ChatMessageContentPartKind.Refusal:
                        break;
                }
            }

            return sbd.ToString();
        }

    }
}


