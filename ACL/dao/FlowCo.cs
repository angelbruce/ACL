using ABL.Data;
using ABL.Object;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ACL.dao
{
    [Table("flow_info")]
    public class FlowInfo : AbstractData
    {
        [Description("编号")]
        [Column("flow_id", IsPK = true, KeyGenerateStrategy = PrimaryKeyGenerateStrategy.AutoIncement)]
        public long Id { get; set; }

        [Column("flow_name")]
        [Description("流程名称")]
        public string Name { get; set; }

        [Column("description")]
        [Description("描述")]
        public string Description { get; set; }
    }

    [Table("flow_config")]
    public class FlowConfig : AbstractData
    {
        [Column("flow_id", IsPK = true, KeyGenerateStrategy = PrimaryKeyGenerateStrategy.None)]
        public long Id { get; set; }

        [Column("flow_desc")]
        public string Desc { get; set; }
    }

    [Table("flow_item")]
    public class FlowItem : AbstractData
    {
        [Description("编号")]
        [Column("flow_item_id", IsPK = true, KeyGenerateStrategy = PrimaryKeyGenerateStrategy.AutoIncement)]
        public long Id { get; set; }


        [Description("编号")]
        [Column("flow_id")]
        public long FlowId { get; set; }

        [Column("flow_item_name")]
        [Description("流程节点名称")]
        public string Name { get; set; }


        [Column("flow_item_type")]
        [Description("流程节点类型")]
        public FlowType FlowType { get; set; }
    }


    public enum FlowType
    {
        /// <summary>
        /// AGENT 执行节点
        /// </summary>
        Agent,
        /// <summary>
        /// 服务调用或函数调用，等待扩展用
        /// </summary>
        Call
    }

    /// <summary>
    /// 流程运行时
    /// </summary>
    [Table("flow_runtime")]
    public class FlowRuntime : AbstractData
    {
        [Description("编号")]
        [Column("flow_runtime_id", IsPK = true, KeyGenerateStrategy = PrimaryKeyGenerateStrategy.AutoIncement)]
        public long Id { get; set; }

        [Description("归属流程编号")]
        [Column("flow_id")]
        public long FlowId { get; set; }

        [Description("归属流程编号")]
        [Column("is_over")]
        public bool IsOver { get; set; }
    }

    /// <summary>
    /// 运行时流程节点
    /// </summary>
    [Table("flow_runtime_node")]
    public class FlowRuntimeNode : AbstractData
    {
        [Description("编号")]
        [Column("flow_runtime_node_id", IsPK = true, KeyGenerateStrategy = PrimaryKeyGenerateStrategy.AutoIncement)]
        public long Id { get; set; }


        [Description("归属流程编号")]
        [Column("flow_id")]
        public long FlowId { get; set; }


        [Description("运行时流程编号")]
        [Column("flow_runtime_id")]
        public long FlowRuntimeId { get; set; }


        [Description("运行时流程编号")]
        [Column("agent_id")]
        public long AgentId { get; set; }

        [Description("运行时流程状态")]
        [Column("status")]
        public NodeStates Status { get; set; } = NodeStates.None;
    }


    public enum NodeStates
    {
        None,
        Running,
        Stop,
    }

    /// <summary>
    /// 运行时流程节点对应动作
    /// </summary>
    [Table("flow_runtime_node_action")]
    public class FlowRuntimeNodeAction : AbstractData
    {
        [Description("编号")]
        [Column("flow_runtime_node_action_id", IsPK = true, KeyGenerateStrategy = PrimaryKeyGenerateStrategy.AutoIncement)]
        public long Id { get; set; }

        [Description("归属流程编号")]
        [Column("flow_id")]
        public long FlowId { get; set; }

        [Description("运行时流程编号")]
        [Column("flow_runtime_id")]
        public long FlowRuntimeId { get; set; }

        [Description("运行时流程节点编号")]
        [Column("flow_runtime_node_id")]
        public long FlowRuntimeNodeId { get; set; }

        [Description("执行动作")]
        [Column("action_id")]
        public long ActionId { get; set; }

        [Description("执行动作")]
        [Column("action_desc")]
        public string Action { get; set; } = string.Empty;

        [Description("动作提示")]
        [Column("action_prompt_id")]
        public long? PromptId { get; set; } = null;

        [Description("动作提示")]
        [Column("action_prompt")]
        public string Prompt { get; set; } = string.Empty;
    }

    public class FlowBody
    {
        public FlowInfo Info { get; set; }

        public List<FlowItem> Items { get; set; }

        public FlowConfig Config { get; set; }
    }

    public class FlowRuntimeBody
    {
        public FlowRuntime Runtime { get; set; }
        public List<FlowRuntimeNodeWithActions> Nodes { get; set; }
        public List<FlowRuntimeNodeAction> Actions { get; set; }
    }

    public class FlowRuntimeNodeWithActions : FlowRuntimeNode
    {
        public List<FlowRuntimeNodeAction> Actions { get; set; }
    }
}
