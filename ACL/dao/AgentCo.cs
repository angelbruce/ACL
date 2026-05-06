using ABL;
using ABL.Data;
using ABL.Object;
using ACL.business.mcp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ACL.dao
{
    /// <summary>
    /// agent
    /// </summary>
    [Table("agent_info")]
    public class AgentInfo : AbstractData
    {
        [Description("编号")]
        [Column(Name = "agent_id", IsPK = true, KeyGenerateStrategy = PrimaryKeyGenerateStrategy.AutoIncement)]
        public long Id { get; set; }



        [Description("名称")]
        [Column("agent_name")]
        public string Name { get; set; }


        [Description("agent设定")]
        [Column("agent_def")]
        public string Defination { get; set; }


    }
    /// <summary>
    /// agent skill
    /// </summary>
    [Table("agent_skill_info")]
    public class AgentSkillInfo : AbstractData
    {
        [Description("编号")]
        [Column(Name = "agent_skill_id", IsPK = true, KeyGenerateStrategy = PrimaryKeyGenerateStrategy.AutoIncement)]
        public long Id { get; set; }


        /// <summary>
        /// agent id
        /// </summary>
        [Column("agent_id")]
        [Description("agent编号")]
        public long AgentId { get; set; }


        [Description("名称")]
        [Column("agent_skill_name")]
        public string Name { get; set; }



        [Description("agent skill设定")]
        [Column("agent_skill_prompt")]
        public string SkillPrompt { get; set; }

    }


    [Table("agent_mcp_tool_info")]
    public class AgentMcpToolInfo : AbstractData
    {
        public AgentMcpToolInfo() { }

        [Field]
        [Column(Name = "agent_mcp_tool_id", IsPK = true, KeyGenerateStrategy = PrimaryKeyGenerateStrategy.AutoIncement)]
        public long Id { get; set; } = 0;

        /// <summary>
        /// agent id
        /// </summary>
        [Column("agent_id")]
        [Field]
        [Description("agent编号")]
        public long AgentId { get; set; } = 0;


        [Description("名称")]
        [Field]
        [Column("fn_name")]
        public string Name { get; set; } = string.Empty;


        [Description("函数用途")]
        [Field]
        [Column("fn_desc")]
        public string Description { get; set; } = string.Empty;


        [Description("输入参数说明")]
        [Field]
        [Column("fn_inputschema")]
        public string InputSchema { get; set; }


        [Description("输出参数说明")]
        [Field]
        [Column("fn_outputschema")]
        public string OutputSchema { get; set; }

    }
}
