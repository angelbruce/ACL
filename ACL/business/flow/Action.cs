using ABL.Enums;
using ABL.Object;
using ACL.dao;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACL.business.flow
{
    class Action
    {
        public string Id { get; set; } = string.Empty;
        public AgentInfo? AgentInfo { get; set; } = null;
        public string Ask { get;  set; } = string.Empty;
        public List<string> Prompts { get; set; } = new List<string>();
        public List<string> NextChoices { get;  set; } = new List<string>();
        public string? DegreeForChoice { get; set; }
        public EnumActionType Type { get; set; } 
    }

    enum EnumActionType
    {
        [Description("start")]
        start,
        [Description("agent")]
        agent,
        [Description("prompt")]
        prompt,
        [Description("ask")]
        ask,
        [Description("terminate")]
        terminate,
        [Description("over")]
        over
    }
}
