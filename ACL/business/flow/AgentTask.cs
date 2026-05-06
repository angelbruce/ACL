using ACL.business.agent;
using ACL.dao;
using ACL.flow;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACL.business.flow
{

    class AgentTask 
    {
        public string Id { get; set; } = string.Empty;
        public AgentBody? Body { get; set; } = null;
        public List<string> Asks { get; private set; } = new List<string>();
        public string Type { get; set; } = string.Empty;
        public IAgent? Agent { get; set; } = null;


        public AgentTask(Action action)
        {
            this.Id = action.Id;
            this.Asks = action.Asks.ToList();
            this.Type = action.Type;
        }
    }

}
