using ACL.business.log;
using McpOrchestrator;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ACL.business.mcp
{

    public class McpSession : IAsyncDisposable
    {
        private MCPToolFactory mcpToolFactory;
        public McpSession()
        {
            this.mcpToolFactory = new MCPToolFactory();
        }

        public async Task<bool> InitializeAsync()
        {
            return await this.mcpToolFactory.InitSessionAsync();
        }
        public async Task<List<MCPTool>> ListToolsAsync()
        {
            return await this.mcpToolFactory.GetToolsAsync();
        }
        public async Task<MCPToolCallResult> CallToolAsync(string tool, BinaryData args)
        {
            return await this.mcpToolFactory.CallToolAsync(tool, args);
        }


        public async ValueTask DisposeAsync()
        {
            await mcpToolFactory.DisposeAsync();
        }
    }


}
