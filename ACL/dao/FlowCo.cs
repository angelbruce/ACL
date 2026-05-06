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
        Agent,
        Call
    }


    public class  FlowBody  {
        public FlowInfo Info {  get; set; }

        public List<FlowItem> Items { get; set; }

        public FlowConfig Config { get; set; }
    }
}
