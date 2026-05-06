using System;
using System.Collections.Generic;
using System.Text;

namespace ACL.business.mcp
{
    public interface IMCPable : IAsyncDisposable
    {
        public bool Test(string prefix);
        public Task<bool> InitializeAsync();
        public Task<bool> SessionInitializeAsync();
        public Task<List<MCPTool>> ListToolsAsync();
        public Task<MCPToolCallResult> CallToolAsync(string toolName, object arguments);
    }
}
