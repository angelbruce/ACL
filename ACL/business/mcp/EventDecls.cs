using ACL.business.agent;
using ACL.business.llm;
using ACL.dao;
using ACL.flow;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACL.business.mcp
{
    public class MCPToolChangedEventArgs : EventArgs
    {
        public List<MCPTool>? Tools { get; internal set; }
    }

    public delegate void DgtMCPToolChanged(MCPToolChangedEventArgs e);


    public class SessionChangedEventArgs : EventArgs
    {
        public Session? Current { get; set; }
        public List<SessionItem>? SessionItems { get; set; }

    }

    public delegate void DgtCurrentSessionChagned(SessionChangedEventArgs e);

    public class SessionItemChangedEventArgs : EventArgs
    {
        public SessionItem? Item { get; set; }
    }


    public delegate void DgtCurrentSessionItemChagned(SessionItemChangedEventArgs e);

    public class CurrentLlmModelInfoChangedEventArgs : EventArgs
    {
        public LLMModelInfo? Current { get; set; }
    }

    public delegate void DgtCurrentLlmModelInfoChagned(CurrentLlmModelInfoChangedEventArgs e);


    public delegate void DgtCurrentPromptUsageChanged(string usage);

    public delegate void DgtSessionItemCreated(TaskInfo step);

    public delegate void DgtTasksChanged(List<TaskInfo> tasks, TaskInfo task);


    public class AgentChangedEventArgs : EventArgs
    {
        public AgentInfo Info { get; set; }
        public IAgent Agent { get; set; }
    }

    public delegate void DgtAgentChanged(AgentChangedEventArgs e);

    public delegate void DgtAgentInfoChanged(AgentChangedEventArgs e);

    public delegate void DgtFileChanged(string file);
}
