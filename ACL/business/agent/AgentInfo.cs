using ACL.dao;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACL.business.agent
{
    public class AgentBody : AgentInfo
    {

        public AgentBody() { }

        public List<AgentSkillInfo>? Skills { get; set; }

        public List<AgentMcpToolInfo> Tools { get; set; }

        public List<ContentStoreConfig> ContentStores { get; set; }
    }
}
