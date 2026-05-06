using ABL.Config.Ant;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACL.business.mcp.dialect
{
    public class StdioMcpConfig
    {
        const string MCP_CONFIG_TAG = "stdio-mcp";
        public static async Task<List<StdioMcpInfo>> GetStdioMcpInfoAsync()
        {
            var list = new List<StdioMcpInfo>();
            var items = AntContext.Instance.GetItems(MCP_CONFIG_TAG);
            if (items != null && items.Count > 0)
            {
                foreach (var item in items)
                {
                    if (item is StdioMcpInfo stdioMcp)
                    {
                        list.Add(stdioMcp);
                    }
                }
            }
            return list;
        }
    }

}