using ACL.business.agent;
using ACL.dao;
using ACL.flow;
using ACL.web;

namespace ACL.business.flow
{
    class Flow
    {
        public const string FLOW_START_NAME = "开始";

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
            if (agents.Head == null)
            {
                throw new InvalidFlowConfigException();
            }

            flow = null;

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

                //construct the graph node from task data info.
                task.Agent = agent;
                var node = graph.AddNode(task);
                if (action.IsHead)
                {
                    node.IsHead = true;
                    graph.Head = node;
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
                if (item.value.Equals(FLOW_START_NAME))
                {
                    startId = item.id;
                    continue;
                }

                if (aid != null)
                {
                    var agentId = aid.Value + "";
                    if (!agentMap.ContainsKey(agentId)) continue;
                    var agent = agentMap[agentId];
                    var node = new Action
                    {
                        Id = item.id,
                        AgentInfo = agent,
                        Type = item.type
                    };
                    var actionNode = graph.AddNode(node);
                    actionMap[item.id] = actionNode;
                    continue;
                }

                others[item.id] = item;
            }

            string? nextId = null;
            if (model.edges != null && model.edges.Count > 0)
            {
                foreach (var edge in model.edges)
                {
                    var src = edge.src;
                    var target = edge.target;

                    if (src == startId)
                    {
                        nextId = target;
                        continue;
                    }

                    var srcPrompts = new List<string>();
                    var fromNode = FindNodeOrPrompts(src, edge, model.edges, others, actionMap, srcPrompts);
                    //悬空的节点不考虑。
                    if (fromNode != null)
                    {
                        fromNode.Data?.Asks.AddRange(srcPrompts);
                    }

                    var targetPrompts = new List<string>();
                    var toNode = FindNodeOrPrompts(target, edge, model.edges, others, actionMap, targetPrompts);
                    //悬空的节点不考虑。
                    if (toNode != null)
                    {
                        toNode.Data?.Asks.AddRange(targetPrompts);
                    }

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

            if (string.IsNullOrEmpty(startId) || string.IsNullOrEmpty(nextId))
            {
                throw new InvalidFlowConfigException();
            }

            foreach (var edge in graph.Edges)
            {
                if (edge?.From?.Data?.Id == nextId)
                {
                    edge.From.IsHead = true;
                    graph.Head = edge.From;
                    break;
                }
            }

            return graph;
        }

        private Node<Action>? FindNodeOrPrompts(string id, Edge e, List<Edge> es, Dictionary<string, Vertex> others, Dictionary<string, Node<Action>> actionMap, List<string> prompts)
        {
            //找到此id
            if (actionMap.ContainsKey(id)) return actionMap[id];

            if (others.ContainsKey(id))
            {
                var vertex = others[id];
                prompts.Insert(0, vertex.value);

                //发起的，找终结点
                if (id == e.src)
                {
                    var edge = es.Where(x => x.target == e.src).FirstOrDefault();
                    if (edge != null)
                    {
                        return FindNodeOrPrompts(edge.src, edge, es, others, actionMap, prompts);
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (id == e.target)
                {
                    return FindNodeOrPrompts(e.src, e, es, others, actionMap, prompts);
                }
            }

            return null;
        }
    }
}
