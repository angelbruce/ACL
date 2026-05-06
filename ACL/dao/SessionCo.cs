using ABL.Data;
using ABL.Object;
using ACL.business.mcp.local;
using System.ComponentModel;

namespace ACL.dao
{
    [Description("会话")]
    [Table("session")]
    public class Session : AbstractData
    {
        [Description("会话编号，代表唯一的会话")]
        [Column("id", IsPK = true, KeyGenerateStrategy = PrimaryKeyGenerateStrategy.AutoIncement)]
        public long Id { get; set; }

        [Description("会话内容")]
        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Description("上级会话")]
        [Column("parent_id")]
        public int ParentId { get; set; }
    }

    public enum SessionType
    {
        Sysetem,
        Assistant,
        User,
        Human,
        FunctionCall
    }

    [Description("会话条目")]
    [Table("session_item")]
    public class SessionItem : AbstractData
    {
        [Column("id", IsPK = true, KeyGenerateStrategy = PrimaryKeyGenerateStrategy.AutoIncement)]
        [Required]
        [Description("会话详细信息条目编号")]
        public long Id { get; set; }

        [Column("session_id")]
        [Required]
        [Description("所属会话编号")]
        public long SessionId { get; set; }

        [Description("会话详细信息条目的详细内容")]
        [Column("description")]
        [Required]
        public string Description { get; set; } = string.Empty;

        [Description("会话详细信息条目的所属类型，只能是Prompt,Assistant,User,FunctionCall其中一个值")]
        [Column("session_type")]
        [Required]
        public SessionType SessionType { get; set; }
    }


    [Table("plan")]
    public class Plan
    {
        [Column("id")]
        [Required]
        public int Id { get; set; }


        [Column("state")]
        [Required]
        public PlanState State { get; set; }


        [Column("parent_id")]
        [Required]
        public int ParentId { get; set; }
    }

    public enum PlanState
    {
        Ready,
        Executing,
        Over,
        Terminated
    }

}
