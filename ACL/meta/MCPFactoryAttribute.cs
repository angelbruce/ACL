using System;
using System.Collections.Generic;
using System.Text;

namespace ACL.meta
{
    internal class MCPFactoryAttribute: Attribute
    {
        public  MCPServerType Type {  get; set; } = MCPServerType.STDIO;
    }

    internal enum MCPServerType
    {
        STDIO,
        SSE
    }
}
