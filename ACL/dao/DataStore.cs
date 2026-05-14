using ABL;
using ACL.business.agent;
using System.Linq.Expressions;

namespace ACL.dao
{

    public class DataStore : SQLiteDataSore
    {
        public DataStore()
        {
        }

        public List<Session> QuerySessions()
        {
            var sessions = Fill<Session>();
            return sessions ?? new List<Session>();
        }

        public bool Save(Session session)
        {
            return base.Save(session) > 0;
        }

        public bool Save(SessionItem session)
        {
            return base.Save(session) > 0;
        }


        public List<SessionItem> QuerySessionItems(long id)
        {
            return Fill<SessionItem>(t => t.SessionId == id);
        }

        public AgentBody? GetAgent(long id)
        {
            var datas = Fill<AgentInfo>(t => t.Id == id);
            if (datas == null || datas.Count == 0) return null;

            var body = new AgentBody();
            datas[0].CopyTo(body);

            var tools = Fill<AgentMcpToolInfo>(t => t.AgentId == id);
            body.Tools = tools;

            var skills = Fill<AgentSkillInfo>(t => t.AgentId == id);
            body.Skills = skills;

            var configs = Fill<ContentStoreConfig>(t => t.AgentId == id);
            body.ContentStores = configs;

            return body;
        }

        public FlowBody GetFlowBody(long id)
        {
            var flow = new FlowBody
            {
                Info = GetFlow(id),
                Items = GetFlowItems(id),
                Config = GetFlowConfig(id)
            };

            return flow;
        }

        public List<FlowItem> GetFlowItems(long flowId)
        {
            var datas = Fill<FlowItem>(t => t.FlowId == flowId);
            if (datas == null || datas.Count == 0) return new List<FlowItem>();

            return datas;
        }

        public FlowConfig GetFlowConfig(long flowId)
        {
            var datas = Fill<FlowConfig>(t => t.Id == flowId);
            if (datas == null || datas.Count == 0) return null;

            return datas[0];
        }


        public FlowRuntimeBody? GetFlowRuntimeBody(long id)
        {
            var flowRuntime = Fetch<FlowRuntime>(x => x.FlowId == id && x.IsOver == false);
            if (flowRuntime == null) return null;

            var runtimeId = flowRuntime.Id;
            var nodes = Fill<FlowRuntimeNode>(t => t.FlowRuntimeId == runtimeId && t.FlowId == id);

            var body = new FlowRuntimeBody
            {
                Runtime = flowRuntime,
                Nodes = nodes,
            };

            return body;
        }

        public FlowInfo GetFlow(long id)
        {
            var datas = Fill<FlowInfo>(t => t.Id == id);
            if (datas == null || datas.Count == 0) return null;

            return datas[0];
        }

        public bool Save(AgentInfo agent)
        {
            return base.Save(agent) > 0;
        }

        public bool Save(AgentMcpToolInfo mcpToolInfo)
        {
            return base.Save(mcpToolInfo) > 0;
        }

        public bool Save(AgentSkillInfo skillInfo)
        {
            return base.Save(skillInfo) > 0;
        }

        public bool Save(ContentStoreConfig contentStoreConfig)
        {
            return base.Save(contentStoreConfig) > 0;
        }
    }
}
