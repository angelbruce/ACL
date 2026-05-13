using ABL;
using ACL.dao;
using System.Threading.Channels;

namespace ACL.business.flow
{
    public class FlowExecutor
    {
        private long flowId;
        private DataStore dataStore;
        private Graph<AgentTask>? flowGraph;
        private FlowRuntime? runtime;
        private FlowAgentRuntime? agentRuntime;
        private Flow? flow;
        private FlowBody? flowBody;
        private FlowRuntimeBody? runtimeBody;
        private Channel<string> output;

        public FlowExecutor(long flowId, Channel<string> output)
        {
            dataStore = new DataStore();
            this.flowId = flowId;
            this.output = output;
        }

        public void Start()
        {
            var state = CanRun();
            if (!state) return;

            StartInternal();
        }

        private void StartInternal()
        {
            flowBody = dataStore.GetFlowBody(flowId);
            var config = flowBody.Config;
            ConfigureFlow(flowBody);

            runtimeBody = dataStore.GetFlowRuntimeBody(flowId);
            if (runtimeBody == null) runtimeBody = CreateRuntime();
            if (runtimeBody.Runtime == null) throw new InvalidFlowException();

            runtime = runtimeBody.Runtime;
            var nodes = runtimeBody?.Nodes;
            if (runtime == null || nodes == null || nodes.Count == 0) throw new InvalidFlowException();

            var runningNodes = nodes.Where(x => x.Status == NodeStates.Running).ToList();
            if (runningNodes.Count == 0)
            {
                StopInternal(runtime);
                return;
            }
        }

        private void ConfigureFlow(FlowBody flowBody)
        {
            if (flow == null)
            {
                flow = new Flow(flowBody);
            }

            if (agentRuntime == null)
            {
                flowGraph = flow.Configure();
                if (flowGraph == null) throw new InvalidFlowConfigException();
                agentRuntime = new FlowAgentRuntime(flowGraph);
            }
        }

        record PairNode(FlowRuntimeNode RuntimeNode, Node<AgentTask> ConfigNode);

        private async void OnAgentTaskFninshed(FlowRuntimeNode current)
        {
            if (current == null) throw new InvalidFlowConfigException();
            if (flowGraph == null) throw new InvalidFlowConfigException();
            if (runtime == null) throw new InvalidFlowConfigException();
            if (runtimeBody == null) throw new InvalidFlowConfigException();
            if (agentRuntime == null) throw new InvalidFlowConfigException();

            //update runtime flow node status form running -> running over

            //将当前节点的状态修改为完成状态
            current.Status = NodeStates.RunningOver;
            current.State = ABL.Object.EnumEntityState.Modified;
            dataStore.Save(current);

            //make a decison which nodes to run next based on the graph, and create new nodes with status running, then save to db
            //需要走到蓝图下一步
            var tos = flowGraph.Edges.Where(x => x.From != null && x.From.Data != null && x.From.Data.Id == current.ActionId.ToString()).Select(x => x.To).ToList();
            foreach (var to in tos)
            {
                //下一步
                if (to == null || to.Data == null) continue;
                //获取完成类型，1表示任意一个上一步完成了就可以走下一步，100表示所有的上一步都完成了才可以走下一步
                var toDegree = to.Data.Degree.ParseTo<long>();
                //获取配置中TO节点的上一步节点
                var configFroms = flowGraph.Edges.Where(x => x.To == to).Select(x => x.From).ToList();
                //错误校验
                if (configFroms == null || configFroms.Count == 0) throw new InvalidFlowConfigException();
                //左连接，找出配置中的上一步和运行时的上一步的关系
                var qry = from cf in configFroms
                          where cf.Data != null && cf.Data.Id != null
                          join runtimeNode in runtimeBody.Nodes on cf?.Data?.Id equals runtimeNode.ActionId.ToString() into joinData
                          from d in joinData.DefaultIfEmpty()
                          where (d.Status == NodeStates.Running || d.Status == NodeStates.RunningOver)
                          select new PairNode(d, cf);
                //因为后续需要根据左连接的结果进行判断，所以将左连接的结果先查询出来，放在内存中进行处理
                var leftJoinNodes = qry.ToList();

                //根据完成类型进行判断是否能进行下一步
                switch (toDegree)
                {
                    case 1L:
                        {
                            await PerformAnyOver(current, to, leftJoinNodes);
                            break;
                        }
                    case 100L:
                        {
                            bool flowControl = await PerfromAllOver(current, to, leftJoinNodes);
                            if (!flowControl)
                            {
                                continue;
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 如果当前节点的下一步对应的上一步骤都完成了,才能执行下一步,否则不能执行下一步
        /// </summary>
        /// <param name="current"></param>
        /// <param name="to"></param>
        /// <param name="leftJoinNodes"></param>
        /// <returns></returns>
        private async Task<bool> PerfromAllOver(FlowRuntimeNode current, Node<AgentTask> to, List<PairNode> leftJoinNodes)
        {
            if (current == null) throw new InvalidFlowConfigException();
            if (to == null) throw new InvalidFlowConfigException();
            if (to.Data == null) throw new InvalidFlowConfigException();
            if (runtime == null) throw new InvalidFlowConfigException();
            if (runtimeBody == null) throw new InvalidFlowConfigException();
            if (agentRuntime == null) throw new InvalidFlowConfigException();
            

            //下一步的上一步是否全部完成
            //获取配置中的所有上一步
            var canContinue = true;
            foreach (var joinNode in leftJoinNodes)
            {
                canContinue = joinNode.RuntimeNode != null && joinNode.RuntimeNode.Status == NodeStates.RunningOver;
                if (!canContinue) break;
            }

            if (!canContinue)
            {
                //不能开启此节点，因为上一步的条件没有满足
                return false;
            }

            //需要开启此流程节点
            var node = new FlowRuntimeNode
            {
                ActionId = to.Data.Id.ParseTo<long>(),
                Action = to.Data.Ask,
                FlowId = flowId,
                FlowRuntimeId = runtime.Id,
                Prompt = string.Join("\n", to.Data.Prompts),
                Status = NodeStates.Running,
                State = ABL.Object.EnumEntityState.Added
            };

            dataStore.Save(node);
            runtimeBody.Nodes.Add(node);

            //修改为完成状态
            var overNodes = leftJoinNodes.Where(x => x.RuntimeNode != null && x.RuntimeNode.Status == NodeStates.RunningOver).Select(x => x.RuntimeNode).ToList();
            foreach (var overNode in overNodes)
            {
                overNode.Status = NodeStates.Stop;
                overNode.NextChoice += $"{node.Id}";
                overNode.State = ABL.Object.EnumEntityState.Modified;
            }

            dataStore.Save(overNodes);
            var runtimeAgent = await agentRuntime.ConfigureAsync(output, current, node);
            AssignRuntimeTaskOver(runtimeAgent);
            return true;
        }

        /// <summary>
        ///  执行任何一个完成了就可以走下一步的逻辑，首先检查是否已经有满足条件的节点在运行了，如果有了，就不创建新的节点了，如果没有，就创建一个新的节点，然后将当前节点修改为完成状态
        /// </summary>
        /// <param name="current"></param>
        /// <param name="to"></param>
        /// <param name="leftJoinNodes"></param>
        /// <returns></returns>
        private async Task PerformAnyOver(FlowRuntimeNode current, Node<AgentTask> to, List<PairNode> leftJoinNodes)
        {
            if (current == null) throw new InvalidFlowConfigException();
            if (to == null) throw new InvalidFlowConfigException();
            if (to.Data == null) throw new InvalidFlowConfigException();
            if (runtime == null) throw new InvalidFlowConfigException();
            if (runtimeBody == null) throw new InvalidFlowConfigException();
            if (agentRuntime == null) throw new InvalidFlowConfigException();

            //本身已经完成了 1 个，满足下一步条件
            //需要开启此流程节点
            //因为任何一个完成都可以开启此节点，所以首先检查节点是否已经存在了，如果存在了，就不创建，否则创建一个新的节点。
            var nextNode = runtimeBody.Nodes.Where(x => x.Id > current.Id && x.Status == NodeStates.Running && x.ActionId.ToString() == to.Data.Id).FirstOrDefault();
            if (nextNode == null)
            {
                nextNode = new FlowRuntimeNode
                {
                    ActionId = to.Data.Id.ParseTo<long>(),
                    Action = to.Data.Ask,
                    FlowId = flowId,
                    FlowRuntimeId = runtime.Id,
                    Prompt = string.Join("\n", to.Data.Prompts),
                    Status = NodeStates.Running,
                    State = ABL.Object.EnumEntityState.Added
                };

                //保存到db
                dataStore.Save(nextNode);
                runtimeBody.Nodes.Add(nextNode);
            }

            //修改为完成状态
            var currentNode = leftJoinNodes.Where(x => x.RuntimeNode != null && x.RuntimeNode.Id == current.Id).Select(x => x.RuntimeNode).First();
            currentNode.Status = NodeStates.Stop;
            currentNode.State = ABL.Object.EnumEntityState.Modified;
            currentNode.NextChoice += $"{nextNode.Id}";
            dataStore.Save(currentNode);
            var runtimeAgent = await agentRuntime.ConfigureAsync(output, currentNode, nextNode);
            AssignRuntimeTaskOver(runtimeAgent);
        }

        void AssignRuntimeTaskOver(FlowRuntimeNodeAgent? runtimeNodeAgent)
        {
            if (runtimeNodeAgent == null) return;
            runtimeNodeAgent.AgentTaskFninshed += OnAgentTaskFninshed;
            runtimeNodeAgent.Start(output);
        }


        /// <summary>
        /// 启动流程，创建runtime和head节点，保存到db
        /// </summary>
        /// <returns></returns>
        private FlowRuntimeBody CreateRuntime()
        {
            var runtimeBody = new FlowRuntimeBody()
            {
                Runtime = new FlowRuntime
                {
                    FlowId = flowId,
                    IsOver = false,
                    State = ABL.Object.EnumEntityState.Added,
                },
                Nodes = new List<FlowRuntimeNode>()
            };

            dataStore.Save(runtimeBody.Runtime);
            runtimeBody.Nodes = CreateNodes(runtimeBody.Runtime);
            dataStore.Save(runtimeBody.Nodes);

            return runtimeBody;
        }

        /// <summary>
        /// create nodes for the head of the graph, and save to db, return the nodes with id generated by db
        /// </summary>
        /// <param name="runtime"></param>
        /// <returns></returns>
        private List<FlowRuntimeNode> CreateNodes(FlowRuntime runtime)
        {
            var nodes = new List<FlowRuntimeNode>();
            if (flowGraph == null) throw new InvalidFlowConfigException();

            foreach (var head in flowGraph.Heads)
            {
                if (head == null) continue;
                if (head.Data == null) continue;

                var node = new FlowRuntimeNode
                {
                    FlowId = flowId,
                    FlowRuntimeId = runtime.Id,
                    ActionId = long.Parse(head.Data.Id),
                    Action = head.Data.Ask,
                    Prompt = string.Join(";", head.Data.Prompts),
                    NextChoice = string.Empty,
                    Status = NodeStates.Running,
                    State = ABL.Object.EnumEntityState.Added,
                };

                nodes.Add(node);
            }

            dataStore.Save(nodes);

            return nodes;
        }


        public void Stop()
        {
            var runtime = GetFlowRuntime();
            if (runtime == null || runtime.IsOver) return;

            StopInternal(runtime);
        }

        private void StopInternal(FlowRuntime runtime)
        {
            runtime.IsOver = true;
            runtime.State = ABL.Object.EnumEntityState.Modified;
            dataStore.Save(runtime);
        }

        public bool CanRun()
        {
            var runtime = this.GetFlowRuntime();
            if (runtime == null) return true;

            if (!runtime.IsOver)
            {
                return false;
            }

            return true;
        }

        private FlowRuntime? GetFlowRuntime()
        {
            return dataStore.Fetch<FlowRuntime>(t => t.Id == flowId && !t.IsOver);
        }
    }
}
