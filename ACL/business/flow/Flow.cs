using ACL.business.agent;
using ACL.dao;
using ACL.flow;
using ABL;
using ACL.web;

namespace ACL.business.flow
{
    class Flow
    {

        private DataStore datastore;
        private FlowBody flow;

        public Flow(FlowBody flow)
        {
            this.flow = flow;
            datastore = new DataStore();
        }


        public Graph<AgentTask> Configure()
        {
            var actions = CreateActions();
            var agents = CreateAgentTask(actions);
            if (agents.Heads == null)
            {
                throw new InvalidFlowConfigException();
            }

            return agents;
        }


        /// <summary>
        /// create agent's graph
        /// </summary>
        /// <param name="actionGraph">the flow config graph</param>
        /// <returns></returns>
        /// <exception cref="FlowDanglingException"></exception>
        /// <exception cref="InvalidFlowException"></exception>
        private Graph<AgentTask> CreateAgentTask(Graph<Action> actionGraph)
        {
            var graph = new Graph<AgentTask>();
            var map = new Dictionary<string, Node<AgentTask>>();

            actionGraph.TraverseVertices(action =>
            {
                if (action == null || action.Data == null || string.IsNullOrEmpty(action.Data.Id))
                    throw new FlowDanglingException();

                var id = action?.Data?.Id;
                if (id == null) throw new FlowDanglingException();

                var agentInfo = action?.Data;
                if (agentInfo == null) throw new FlowDanglingException();


                //create task info
                var task = new AgentTask(agentInfo);

                //get agent from agent map if exists or create a new agent from agent body
                Agent? agent = null;
                var agentId = action?.Data?.AgentInfo?.Id;
                if (agentId == null) throw new InvalidFlowException();


                var agentBody = datastore.GetAgent(agentId.Value);
                if (agentBody == null) throw new InvalidFlowException();

                task.Body = agentBody;
                agent = new Agent(task);

                task.Degree = action?.Data?.DegreeForChoice;
                task.Choices = action?.Data?.NextChoices;
                task.Prompts = action?.Data?.Prompts;
                //construct the graph node from task data info.
                task.Agent = agent;
                var node = graph.AddNode(task);
                if (action.IsHead)
                {
                    node.IsHead = true;
                    graph.Heads.Add(node);
                }
                //map the action graph node id to new added graph node to construct graph edge.
                map[id] = node;
            });

            actionGraph.TraverseEdge(action =>
            {
                var fromNode = action.From;
                var toNode = action.To;
                if (fromNode == null || toNode == null) throw new FlowDanglingException();
                if (fromNode.Data == null || toNode.Data == null) throw new FlowDanglingException();

                var fromId = fromNode.Data?.Id;
                var toId = toNode.Data?.Id;
                if (string.IsNullOrEmpty(fromId) || string.IsNullOrEmpty(toId)) throw new FlowDanglingException();

                if (!map.ContainsKey(fromId) || !map.ContainsKey(toId)) throw new FlowDanglingException();

                var edge = new Edge<AgentTask>() { From = map[fromId], To = map[toId] };
                graph.AddEdge(edge);
            });

            return graph;
        }

        /// <summary>
        /// create action graph from flow's config
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidFlowConfigException"></exception>
        private Graph<Action> CreateActions()
        {
            //get mode from agent config
            var agents = datastore.Fill<AgentInfo>();
            var agentMap = agents.ToDictionary(x => x.Id + "", y => y);

            var config = flow.Config;
            if (config == null) throw new InvalidFlowConfigException();

            var description = config.Desc;
            if (description == null || description.Length == 0) throw new InvalidFlowConfigException();

            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<Model>(description);
            if (model == null || model.vertices == null || model.vertices.Count == 0) throw new InvalidFlowConfigException();

            //construct graph
            var graph = new Graph<Action>();
            var actionMap = new Dictionary<string, Node<Action>>();

            var others = new Dictionary<string, Vertex>();
            string? startId = null;
            foreach (var item in model.vertices)
            {
                var aid = item.agent;

                var node = new Action
                {
                    Id = item.id,
                    Type = item.type.ParseTo<EnumActionType>(),
                };

                switch (node.Type)
                {
                    case EnumActionType.start:
                        startId = item.id;
                        break;
                    case EnumActionType.terminate:
                    case EnumActionType.over:
                        continue;
                }

                //没有执行者
                if (aid == null) throw new InvalidFlowConfigException();
                var agentId = aid.Value + "";

                if (!agentMap.ContainsKey(agentId)) throw new InvalidFlowConfigException();

                var agent = agentMap[agentId];
                node.AgentInfo = agent;
                if (!string.IsNullOrEmpty(item.prompt))
                {
                    node.Prompts = new List<string>() { item.prompt };
                }

                node.NextChoices = new List<string>();
                if (item.paths != null && item.paths.Count > 0)
                {
                    node.NextChoices.AddRange(item.paths);
                }

                node.DegreeForChoice = item.degree;

                var actionNode = graph.AddNode(node);
                actionMap[item.id] = actionNode;
            }


            var nextIds = new HashSet<string>();
            if (model.edges != null && model.edges.Count > 0)
            {
                foreach (var edge in model.edges)
                {
                    var val = edge.value;
                    if (string.IsNullOrEmpty(val)) throw new InvalidFlowConfigException();

                    var src = edge.src;
                    var target = edge.target;

                    if (src == startId)
                    {
                        nextIds.Add(target);
                        continue;
                    }

                    var fromNode = FindNode(src, actionMap);
                    var toNode = FindNode(target, actionMap);
                    if (fromNode != null && toNode != null)
                    {
                        var actionEdge = new Edge<Action>
                        {
                            From = fromNode,
                            To = toNode
                        };

                        graph.AddEdge(actionEdge);
                    }
                }
            }

            if (string.IsNullOrEmpty(startId) || nextIds.Count == 0)
            {
                throw new InvalidFlowConfigException();
            }

            foreach (var edge in graph.Edges)
            {
                if (nextIds.Contains(edge?.From?.Data?.Id))
                {
                    edge.From.IsHead = true;
                    graph.Heads.Add(edge.From);
                    break;
                }
            }

            return graph;
        }


        private Node<Action>? FindNode(string id, Dictionary<string, Node<Action>> actionMap)
        {
            //找到此id
            if (actionMap.ContainsKey(id)) return actionMap[id];

            return null;
        }
    }
}
