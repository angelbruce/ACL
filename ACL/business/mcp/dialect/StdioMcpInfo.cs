using ABL.Config.Ant;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACL.business.mcp.dialect
{
    public class StdioMcpAnt : BaseAnt
    {
        public override IAntItem Create()
        {
            return new StdioMcpInfo();
        }
    }


    [AntRoot("stdio-mcp-collection")]
    [AntPrefix("stdio-mcp")]
    public class StdioMcpInfo : IAntItem
    {
        [AntElement("name")]
        public string? Name { get; set; }

        [AntElement("command")]
        public string? Command { get; set; }

        [AntElementCollection("args")]
        public List<StdioMcpInfoArg>? Args { get; set; }

        [AntElementCollection("passed-tools")]
        public List<StdioMcpDisabledTool>? PassedTools { get; set; }
    }

    [AntPrefix("arg")]
    public class StdioMcpInfoArg : IAntItem
    {
        [AntElement("name")]
        public string Name { get; set; }
    }

    [AntPrefix("tool")]
    public class StdioMcpDisabledTool : IAntItem
    {
        [AntElement("name")]
        public string Name { get; set; }
    }

}