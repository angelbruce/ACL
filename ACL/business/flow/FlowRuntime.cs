using ACL.business.mcp;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Channels;

namespace ACL.business.flow
{
    class FlowRuntime
    {
        private readonly Graph<AgentTask> blueprint;
        private List<Node<AgentTask>> currents;

        private volatile bool running;
        public bool Running { get { return running; } }

        public FlowRuntime(Graph<AgentTask> blueprint)
        {
            currents = new List<Node<AgentTask>>();
            this.blueprint = blueprint;
        }


        public void Configure(Channel<string> output)
        {
            if (running) return;

            if (blueprint == null || blueprint.Head == null)
            {
                throw new InvalidFlowConfigException();
            }

            //头部不参与计算
            var head = blueprint.Head;
            var nodes = new List<Node<AgentTask>>() { head };
            currents.AddRange(nodes);

            var channelMap = new Dictionary<string, Channel<string>>();
            //启动所有AGENT，保证所有活动链接及时响应。
            blueprint.TraverseVertices(node =>
            {
                var task = node.Data;
                if (task == null) return;

                var agent = task.Agent;
                if (agent == null) return;

                task.AgentTaskFninshed += OnAgentTaskFinished;
                agent?.Serve(output);
            });
        }


        /// <summary>
        /// 什么时候停止？
        /// action action
        /// a(in, out) b(in, out)
        /// ANSWER: IF ALL TASK OF THE AGENT'S PLANS FINISHED.
        /// </summary>
        /// <param name="agent"></param>
        private async void OnAgentTaskFinished(ACL.flow.Agent agent)
        {
            if (currents == null) return;
            //任何节点没有完成，就等待。

            var list = new List<Node<AgentTask>>(currents);
            currents.Clear();

            //所有参与节点都结束，就进入到下一环节。
            //先考虑简单流程，不考虑复杂的，TODO: 复杂流程等待中.....
            foreach (var from in list)
            {
                var tos = from.ToNodes;
                if (tos == null || tos.Count == 0) continue;

                foreach (var to in tos)
                {
                    currents.Add(to);
                    await AssignSession(from?.Data, to?.Data);
                }
            }
        }

        private async Task<bool> AssignSession(AgentTask? from, AgentTask? to)
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

        public void Start()
        {
            if (running) return;

            if (currents == null || currents.Count == 0)
            {
                throw new InvalidFlowException();
            }

            running = true;

            return;
        }

        public void Stop()
        {
            running = false;
        }
    }
}
