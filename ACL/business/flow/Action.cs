using ABL.Object;
using ACL.dao;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACL.business.flow
{
    class Action : AbstractData
    {
        public string Id { get; set; } = string.Empty;
        public AgentInfo? AgentInfo { get; set; } = null;
        public List<string> Asks { get; private set; } = new List<string>();
        public string Type { get; set; } = string.Empty;
    }
}
