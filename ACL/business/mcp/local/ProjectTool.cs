using ACL.business.project;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ACL.business.mcp.local
{
    [McpServerTool]
    public class ProjectTool
    {
        [McpTool, Description("获取当前项目所在目录")]
        public static ProjectConfigInfo GetCurrentProject()
        {
            return ProjectConfig.Current;
        }
    }
}
