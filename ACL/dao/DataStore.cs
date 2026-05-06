using ABL;
using ACL.business.agent;

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
            return Fill<SessionItem>("select * from session_item where session_id=" + id);
        }

        public AgentBody? GetAgent(long id)
        {
            var datas = Fill<AgentInfo>($"select * from agent_info where agent_id={id}");
            if (datas == null || datas.Count == 0) return null;

            var body = new AgentBody();
            datas[0].CopyTo(body);

            var tools = Fill<AgentMcpToolInfo>($"select * from agent_mcp_tool_info where agent_id='{id}'");
            body.Tools = tools;

            var skills = Fill<AgentSkillInfo>($"select * from agent_skill_info where agent_id='{id}'");
            body.Skills = skills;

            var configs = Fill<ContentStoreConfig>($"select * from agent_content_store_config where agent_id='{id}'");
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
            var datas = Fill<FlowItem>($"select * from flow_item where flow_id={flowId}");
            if (datas == null || datas.Count == 0) return new List<FlowItem>();

            return datas;
        }

        public FlowConfig GetFlowConfig(long flowId)
        {
            var datas = Fill<FlowConfig>($"select * from flow_config where flow_id='{flowId}'");
            if (datas == null || datas.Count == 0) return null;

            return datas[0];
        }

        public FlowInfo GetFlow(long id)
        {
            var datas = Fill<FlowInfo>($"select * from flow_info where flow_id={id}");
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
