using ABL;
using ACL.business.agent;
using ACL.business.llm;
using ACL.business.mcp;
using ACL.business.project;
using ACL.business.prompt;
using ACL.dao;
using ACL.flow;

namespace ACL.business
{
    public class Context
    {
        private static readonly Context context = new Context();

        public event DgtCurrentSessionChagned? CurrentSessionChagned;
        public event DgtCurrentLlmModelInfoChagned? CurrentLlmModelInfoChagned;
        public event DgtCurrentPromptUsageChanged? CurrentPromptUsageChanged;
        public event DgtSessionItemCreated? SessionItemCreated;
        public event DgtTasksChanged? TaskChanged;
        public event DgtMCPToolChanged? McpToolChanged;
        public event DgtAgentChanged AgentChanged;
        public event DgtAgentInfoChanged AgentRefreshed;

        private Session? session;
        private LLMModelInfo? modelInfo;
        private List<Prompt>? allPrompts;
        private List<TaskInfo> tasks;
        private List<MCPTool>? tools;

        private McpSession mcpSession;

        private IAgent agent;
        private AgentInfo currentAgent;
        public IAgent Agent { get { return agent; } }

        public event DgtFileChanged FileChanged;

        public AgentInfo CurrentAgent
        {
            get { return currentAgent; }
            set
            {
                if (value == null) return;

                var store = new DataStore();
                var agentBody = store.GetAgent(value.Id);
                if (agentBody == null) return;

                var refresh = currentAgent != null && currentAgent.Id == value.Id;
                currentAgent = value;
                if (refresh)
                {

                    if (AgentRefreshed != null)
                    {
                        var e = new AgentChangedEventArgs
                        {
                            Info = currentAgent,
                            Agent = agent
                        };

                        AgentRefreshed(e);
                    }
                }
                else
                {
                    if (agent != null)
                    {
                        agent.Dispose();
                        agent = null;
                    }

                    agent = new SessionAgent(agentBody);

                    if (AgentChanged != null)
                    {
                        var e = new AgentChangedEventArgs
                        {
                            Info = currentAgent,
                            Agent = agent
                        };

                        AgentChanged(e);
                    }
                }
            }
        }

        private Context()
        {
            tasks = new List<TaskInfo>();
            mcpSession = new McpSession();

        }

        public async Task<bool> InitializeSession()
        {
            await mcpSession.InitializeAsync();
            MCPTools = await mcpSession.ListToolsAsync();
            return await Task.FromResult(true);
        }

        public static Context Instance { get { return context; } }

        public List<MCPTool>? MCPTools
        {
            get { return tools; }
            private set
            {
                tools = value;
                var e = new MCPToolChangedEventArgs() { Tools = tools?.ToList() };
                if (McpToolChanged != null)
                {
                    McpToolChanged(e);
                }
            }
        }

        public List<TaskInfo>? Tasks
        {
            get { return tasks; }
        }

        public async Task<MCPToolCallResult> CallToolAsync(string fnName, BinaryData parameters)
        {
            return await mcpSession.CallToolAsync(fnName, parameters);
        }

        private void StoreOutput(TaskInfo? task)
        {
            if (task == null) return;

            if (!string.IsNullOrEmpty(task.OutputResult) && !string.IsNullOrEmpty(task.OutFile))
            {
                var file = Path.Combine(ProjectConfig.Current.Directory, task.OutFile);
                if (!File.Exists(file))
                {
                    var dir = Path.GetDirectoryName(file);
                    if (!Directory.Exists(dir))
                    {
                        var d = Directory.CreateDirectory(dir);
                        if (!d.Exists)
                        {
                            Thread.Sleep(0);
                        }
                    }
                }


                File.WriteAllText(file, FormatContent(task.OutputResult));
                if (FileChanged != null)
                {
                    FileChanged(file);
                }
            }

            var tasks = task.SubTaskList;
            if (tasks != null && tasks.Count > 0)
            {
                foreach (var taskInfo in tasks)
                {
                    StoreOutput(taskInfo);
                }
            }
        }

        private string FormatContent(string output)
        {
            if (output == null) return output;

            return output.Replace("\\n", "\n");
        }

        public void RefreshTask(TaskInfo? task)
        {
            if (task == null) { return; }
            StoreOutput(task);

            if (!Refresh(tasks, task))
            {
                tasks.Add(task);
            }

            if (TaskChanged != null)
            {
                TaskChanged(tasks, task);
            }
        }

        private bool Refresh(List<TaskInfo> containers, TaskInfo task)
        {
            for (int i = 0; i < containers.Count; i++)
            {
                var tk = containers[i];
                if (tk.Id == task.Id)
                {
                    tasks.RemoveAt(i);
                    tasks.Insert(i, tk);
                    return true;
                }


                var subs = tk.SubTaskList;
                if (subs != null)
                {
                    if (Refresh(subs, task))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public List<Prompt> AllPrompts
        {
            get
            {
                if (allPrompts == null)
                {
                    var promptConfig = new PromptConfig();
                    allPrompts = promptConfig.Prompts() ?? new List<Prompt>();
                }

                return this.allPrompts;
            }
        }


        private string usage = string.Empty;
        public string CurrentUsage
        {
            get { return usage; }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (usage != value)
                {
                    usage = value;
                    if (CurrentPromptUsageChanged != null)
                    {
                        CurrentPromptUsageChanged(usage);
                    }
                }
            }
        }


        public List<Prompt> Prompts
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentUsage))
                {
                    return new List<Prompt>();
                }

                return AllPrompts.Where(x => x.Usage == CurrentUsage).ToList();
            }
        }

        public LLMModelInfo? CurrentModel
        {
            get
            {
                if (modelInfo == null)
                {
                    var task = ModelConfig.Instance.Current();
                    modelInfo = task.Result;

                    if (CurrentLlmModelInfoChagned != null)
                    {
                        CurrentLlmModelInfoChagned(new CurrentLlmModelInfoChangedEventArgs
                        {
                            Current = modelInfo,
                        });
                    }
                }

                return modelInfo;
            }
            set
            {
                var isChanged = false;
                if (modelInfo == null && value != null)
                {
                    isChanged = true;
                    modelInfo = value;
                }
                else if (session != null && value != null && modelInfo != value)
                {
                    isChanged = true;
                    modelInfo = value;
                }

                if (isChanged)
                {
                    if (CurrentLlmModelInfoChagned != null)
                    {
                        CurrentLlmModelInfoChagned(new CurrentLlmModelInfoChangedEventArgs
                        {
                            Current = value,
                        });
                    }
                }
            }
        }

        public Session? CurrentSession
        {
            get { return session; }
            set
            {
                var isChanged = false;
                if (session == null && value != null)
                {
                    isChanged = true;
                    session = value;
                }
                else if (session != null && value != null && session != value)
                {
                    isChanged = true;
                    session = value;
                }

                if (isChanged && value != null)
                {
                    if (CurrentSessionChagned != null)
                    {
                        var store = new DataStore();
                        var items = store.QuerySessionItems(value.Id);
                        if (items == null) items = new List<SessionItem>();
                        items.Add(new SessionItem { Description = "目标：" + session.Description, SessionType = SessionType.Sysetem });
                        CurrentSessionChagned(new SessionChangedEventArgs
                        {
                            Current = value,
                            SessionItems = items
                        });
                    }
                }
            }
        }

    }
}
