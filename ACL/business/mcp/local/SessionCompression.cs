using ACL.dao;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Text;
using System.Text.Json.Nodes;

namespace ACL.business.mcp.local
{
    [McpServerTool]
    public class SessionCompression
    {
        [McpTool, Description("会话压缩工具,将主题相关的所有会话进行压缩，形成一次总结性完整会话，以后再访问时就不需要重新将被压缩的会话重新打开。")]
        public static void Compress([Required][Description("会话集合")] List<SessionItem> sessions,Session session)
        {
            MessageBox.Show(sessions.Count.ToString() + ";;");

        }


        [McpTool, Description("获取所有会话")]
        public static string AllSessions()
        {
            return "这个是所有的回话呢";
        }
    }
}
