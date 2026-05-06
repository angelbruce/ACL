using ACL.dao;
using OpenAI.Chat;
using System.Text;
using System.Threading.Channels;

namespace ACL.business.session
{
    public class SessionStep
    {
        private Channel<ChatMessage> channel;
        private bool running = false;
        private CancellationTokenSource ctsPlan;
        private CancellationTokenSource ctsPerform;

        public List<ChatMessage> Messages { get; set; }
        public Session Session { get; internal set; }
        public int Index { get; set; }


        public SessionStep(Session session, List<ChatMessage> messages)
        {
            Session = session;
            this.Messages = messages;
            this.Index = messages.Count;
            ctsPlan = new CancellationTokenSource();
            ctsPerform = new CancellationTokenSource();
            channel = Channel.CreateBounded<ChatMessage>(10);
        }
        public void Stop()
        {
            running = false;
            ctsPlan.Cancel();
            ctsPerform.Cancel();

        }

        public void Perform()
        {
            if (running) return;
            running = true;

            Plan();
            Store();
        }

        private void Store()
        {
            Task.Run(async () =>
            {
                var store = new DataStore();
                while (running)
                {
                    var chat = await channel.Reader.ReadAsync();
                    var item = new SessionItem
                    {
                        Id = 0,
                        SessionId = Session.Id,
                        Description = GetText(chat),
                    };

                    var type = chat.GetType();
                    if (type == typeof(SystemChatMessage))
                    {
                        item.SessionType = SessionType.Sysetem;
                    }
                    else if (type == typeof(AssistantChatMessage))
                    {
                        item.SessionType = SessionType.Assistant;
                    }
                    else if (type == typeof(UserChatMessage))
                    {
                        item.SessionType = SessionType.User;
                    }
                    else if (type == typeof(ToolChatMessage))
                    {
                        item.SessionType = SessionType.FunctionCall;
                    }
                    else
                    {
                        item.SessionType = SessionType.Human;
                    }

                    item.State = ABL.Object.EnumEntityState.Added;

                    store.Save(item);
                }
            }, ctsPerform.Token);
        }

        private string GetText(ChatMessage chat)
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

        private void Plan()
        {
            Task.Run(async () =>
            {
                do
                {
                    int len = Messages.Count;
                    while (Index < len - 1)
                    {
                        ChatMessage message = Messages[Index];
                        await channel.Writer.WriteAsync(message);
                        Index ++;
                    }
                    Thread.Sleep(10000);
                } while (running);
            }, ctsPlan.Token);
        }
    }
}
