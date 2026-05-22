using ACL.business.log;
using OpenAI.Assistants;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Channels;

namespace ACL.business.session
{
    public delegate void DgtPostMessageReceived(Message message);
    public class ChannelMessage<T>
    {

        private Channel<T> channel;
        public ChannelMessage()
        {
            channel = Channel.CreateUnbounded<T>();
        }

        public async ValueTask Write(T t, CancellationToken token)
        {
            await channel.Writer.WriteAsync(t, token);
        }

        public async Task<T> Read(CancellationToken token)
        {
            return await channel.Reader.ReadAsync(token);
        }
    }

    public enum Message
    {
        AIWorking,
        AIAnswerOver,
        AICompressing,
        AICompressed,
    }


    public class Instance<T> where T : new()
    {
        private readonly static T t = new T();
        public static T Data { get { return t; } }
    }

    public class PostOffice
    {
        private ChannelMessage<Message> channel;
        private bool run = false;
        public event DgtPostMessageReceived? OnMessageReceived;
        public PostOffice()
        {
            channel = new ChannelMessage<Message>();
        }

        public async ValueTask Post(Message message)
        {
            var cts = new CancellationTokenSource(3000);
            await channel.Write(message, cts.Token);
        }

        public void Start()
        {
            if (run) return;
            run = true;
            Receive();
        }

        public void Stop()
        {
            run = false;
        }

        private void Receive()
        {
            Task.Run(async () =>
            {
                while (run)
                {
                    try
                    {
                        var cts = new CancellationTokenSource(30000);
                        var data = await channel.Read(cts.Token);
                        OnMessageReceived?.Invoke(data);
                        Thread.Sleep(30);
                    }
                    catch (OperationCanceledException)
                    {
                        // PASSED
                    }
                    catch (Exception e)
                    {
                        //OTHERS output
                        GlobalLogger.Error(e.Message);
                    }

                }
            });
        }
    }
}
