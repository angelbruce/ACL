using ACL.business.mcp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace ACL.business.agent
{
    public interface IAgent : IDisposable
    {
        public void OnPromptChanged();
        public void OnMcpToolsChanged();
        public void OnCurrentSessionChanged(SessionChangedEventArgs e);
        public Task RunOrchestrator();
        public Task Serve(Channel<string> channel);
        public Task SetOutput(Channel<string> channel);
        public Task<bool> Chat(string data);
        public void Cancel();
    }
}
