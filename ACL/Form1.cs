using ACL.business;
using ACL.business.llm;
using ACL.business.log;
using ACL.business.mcp;
using ACL.business.project;
using ACL.dao;
using ACL.flow;
using ACL.uc;
using McpOrchestrator;

namespace ACL
{
    public partial class Form1 : Form
    {
        private SessionDataList sessionDataList;
        private ProjectDataList projectDataList;
        private ucAgentDataList agentDataList;
        private ucFlowDataList flowDataList;

        public Form1()
        {
            InitializeComponent();

            new LlamaServer();

            sessionDataList = new SessionDataList();
            projectDataList = new ProjectDataList();
            agentDataList = new ucAgentDataList() { Dock = DockStyle.Fill };
            flowDataList = new ucFlowDataList() { Dock = DockStyle.Fill };

            GlobalLogger.Log = new UILog(txtDebug);
            Context.Instance.McpToolChanged += OnMcpToolsChanged;
            Context.Instance.CurrentSessionChagned += OnSessionChanged;

            sessionDataList.Dock = DockStyle.Fill;
            sessionDataList.Content = this.content;
            this.tabSession.Controls.Add(sessionDataList);

            projectDataList.Dock = DockStyle.Fill;
            projectDataList.tabContent = this.tabContent;
            this.tabProject.Controls.Add(projectDataList);
            this.tabAgent.Controls.Add(agentDataList);
            splitContainer2.Panel1.Controls.Add(flowDataList);
            flowDataList.PanelContainer = panelFlowInfo;
            flowDataList.PanelOutput = pnlOutput;

            this.Load += OnFormLoaded;

            this.FormClosed += OnMainFormClosed;
            this.Shown += OnShown;

            ShowLLMModels();
        }

        private void OnShown(object? sender, EventArgs e)
        {
            LoadLast();
        }

        private void OnMainFormClosed(object? sender, FormClosedEventArgs e)
        {
            LastUsedRecorder.SaveLastInfo();
        }

        private void OnSessionChanged(SessionChangedEventArgs e)
        {
            lblSession.Text = e.Current?.Description;
        }

        private void OnFormLoaded(object? sender, EventArgs e)
        {
            LoadModel();
            //LoadPrompts();
            LoadAgents();
            Task.Run(async () =>
            {
                await Context.Instance.InitializeSession();
            });
        }

        private void LoadLast()
        {
            var info = LastUsedRecorder.GetLastUsed();
            if (!string.IsNullOrEmpty(info.SessionId))
            {
                sessionDataList.SelectedSession = info.SessionId;
            }

            if (!string.IsNullOrEmpty(info.AgentId))
            {
                foreach (ToolStripItem item in tsbAgent.DropDownItems)
                {
                    if (item == null) continue;

                    var tag = item.Tag as AgentInfo;
                    if (tag == null) continue;

                    if (tag.Id.ToString().Equals(info.AgentId))
                    {
                        tsbAgent.Text = tag.Name;
                        Context.Instance.CurrentAgent = tag;
                    }
                }
            }

            if (!string.IsNullOrEmpty(info.LLM))
            {
                foreach (ToolStripItem item in tsdModels.DropDownItems)
                {
                    if (item == null) continue;

                    var tag = item.Tag as LLMModelInfo;
                    if (tag == null) continue;

                    if (tag.Name.ToString().Equals(info.LLM))
                    {
                        tsdModels.Text = tag.Name;
                        Context.Instance.CurrentModel = tag;
                    }
                }
            }
        }

        private void LoadAgents()
        {
            var datastore = new DataStore();
            var agents = datastore.Fill<AgentInfo>();
            foreach (var agent in agents)
            {
                var item = tsbAgent.DropDownItems.Add(agent.Name);
                item.Tag = agent;
            }

            if (agents.Count > 0)
            {
                tsbAgent.Text = agents[0].Name;
                Context.Instance.CurrentAgent = agents[0];
            }
        }

        private void LoadModel()
        {
            var model = Context.Instance.CurrentModel;
            txtModel.Text = string.Empty;
            if (model != null)
            {
                txtModel.Text = model.Name;
            }
        }

        private void OnMcpToolsChanged(MCPToolChangedEventArgs e)
        {
            var tools = e.Tools ?? new List<MCPTool>();
            tvMcpTools.Invoke(() =>
            {
                tvMcpTools.Nodes.Clear();
                foreach (var item in tools)
                {
                    var node = new TreeNode() { Text = item.Name, };
                    var desc = new TreeNode() { Text = item.Description, };
                    var schemas = new TreeNode() { Text = item.InputSchema.ToString(), };
                    node.ToolTipText = item.Description + Environment.NewLine + item.InputSchema.ToString();
                    node.Nodes.Add(desc);
                    node.Nodes.Add(schemas);
                    tvMcpTools.Nodes.Add(node);
                }

                tvMcpTools.Refresh();
            });
        }

        private void OnChoseUsage(object sender, ToolStripItemClickedEventArgs e)
        {
            var usage = e.ClickedItem?.Text;
            if (usage == null || usage.Length == 0) return;

            cmbUsage.Text = usage;
            Context.Instance.CurrentUsage = usage;
        }

        private void tsbAgent_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var agentName = e.ClickedItem?.Text;
            if (agentName == null || agentName.Length == 0) return;

            tsbAgent.Text = agentName;
            var data = e.ClickedItem.Tag as AgentInfo;
            Context.Instance.CurrentAgent = data;
        }

        private void tsbModel_Click(object sender, EventArgs e)
        {
            var model = new ucModels();
            model.FormClosed += OnRefershModels;
            model.ShowDialog();
        }

        private void ShowLLMModels()
        {
            tsdModels.DropDownItems.Clear();
            var factory = new LLMModelFactory();
            var models = factory.GetModelsAsync().Result;
            foreach (var model in models)
            {
                var item = tsdModels.DropDownItems.Add(model.Name);
                item.ToolTipText = string.IsNullOrEmpty(model.Description) ? model.Name : model.Description;
                item.Tag = model;
                item.Click += (s, e) =>
                {
                    tsdModels.Text = model.Name;
                    Context.Instance.CurrentModel = model;
                };

                if (!string.IsNullOrEmpty(model.IsDefault) && model.IsDefault.Equals("1"))
                {
                    tsdModels.Text = model.Name;
                    Context.Instance.CurrentModel = model;
                }
            }
        }

        private void OnRefershModels(object? sender, FormClosedEventArgs e)
        {
            ShowLLMModels();
        }
    }
}
