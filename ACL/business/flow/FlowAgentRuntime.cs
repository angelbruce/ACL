using ACL.business.mcp;
using ACL.dao;
using ACL.flow;
using System.Threading.Channels;

namespace ACL.business.flow
{

    /// <summary>
    /// 流程运行时
    /// </summary>
    class FlowAgentRuntime
    {
        private readonly Graph<AgentTask> blueprint;
        private Dictionary<long, FlowRuntimeNodeAgent> flowAgents;

        public FlowAgentRuntime(Graph<AgentTask> blueprint)
        {
            this.blueprint = blueprint;
            flowAgents = new Dictionary<long, FlowRuntimeNodeAgent>();
        }


        public async Task<FlowRuntimeNodeAgent?> ConfigureAsync(Channel<string> output,
            FlowRuntimeNode current,
            FlowRuntimeNode next
            )
        {
            var fromAgent = FetchOrInitAgent(current);
            var nextAgent = FetchOrInitAgent(next);
            if (fromAgent != null && nextAgent != null)
            {
                await AssignSession(fromAgent, nextAgent);
            }

            if (fromAgent != null)
            {
                Release(fromAgent);
            }

            if (nextAgent != null)
            {
                return nextAgent;
            }

            return null;
        }

        private void Release(FlowRuntimeNodeAgent node)
        {
            if (node == null) return;
            if (node.Node == null) return;

            var id = node.Node.Id;
            if (!flowAgents.ContainsKey(id)) return;

            var data = flowAgents[id];
            if (data == null) return;

            var agent = data.Agent;
            if (agent != null)
            {
                agent.Stop();
            }

            flowAgents.Remove(id);

        }

        private FlowRuntimeNodeAgent? FetchOrInitAgent(FlowRuntimeNode node)
        {
            if (node == null) return null;

            var id = node.Id;
            if (flowAgents.ContainsKey(id) && flowAgents[id] != null)
            {
                return flowAgents[id];
            }

            foreach (var agentTask in blueprint.Nodes)
            {
                if (agentTask?.Data?.Id == node.ActionId.ToString())
                {
                    var task = agentTask.Data;

                    var runtimeAgent = new FlowRuntimeNodeAgent
                    {
                        Node = node,
                        AgentTask = task,
                    };

                    runtimeAgent.Agent = new Agent(runtimeAgent);

                    return runtimeAgent;
                }
            }

            throw new InvalidFlowConfigException();
        }

        private async Task<bool> AssignSession(FlowRuntimeNodeAgent? from, FlowRuntimeNodeAgent? to)
        {
            if (to == null) return false;
            if (from == null) return false;
            // 将会话从from传递到to中 from->to，激活to
            // 此处有多个选项.先按照整体来
            // 1 整体 可以摘要压缩？可以做成流程选项。
            // 2 成果
            // 3 问题
            return await Task.Run(async () =>
            {
                var e = new SessionChangedEventArgs { SessionItems = from.SessionItems };
                to?.Agent?.OnCurrentSessionChanged(e);
                return true;
            });
        }


    }
}
