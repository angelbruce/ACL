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
        /// <summary>
        /// provide the agent service which the text will be write throught the output channel
        /// </summary>
        /// <param name="channel">the output channel</param>
        /// <returns></returns>
        public Task<bool> Chat(string data);
        public void Cancel();
    }
}
