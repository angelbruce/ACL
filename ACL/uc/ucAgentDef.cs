using ABL;
using ABL.Enums;
using ABL.Object;
using ABL.Store;
using ACL.business;
using ACL.business.agent;
using ACL.business.mcp;
using ACL.dao;
using Microsoft.ApplicationInsights.Extensibility;
using OpenAI.Realtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace ACL.uc
{
    public partial class ucAgentDef : Form
    {
        public ucAgentDef()
        {
            InitializeComponent();
            Context.Instance.McpToolChanged += OnMcpToolChanged;
            this.Load += UcAgentDef_Load;
        }

        private AgentBody agent;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AgentBody Agent
        {
            set
            {
                if (value == null) return;
                agent = value;
            }
        }

        private void ShowAgent(AgentBody agent)
        {
            if (agent == null) return;
            txtName.Text = agent.Name;
            txtDef.Text = agent.Defination;
            ShowSkills();
            var tools = Context.Instance.MCPTools;
            ShowTools(tools ?? new List<MCPTool>());
            ShowContentConfigs();
        }

        private void ShowContentConfigs()
        {
            var configs = agent.ContentStores;
            if (configs == null || configs.Count == 0) return;
            foreach (var config in configs)
            {
                tvConfigs.Nodes.Add(new TreeNode
                {
                    Text = config.Name,
                    Tag = config,
                });
            }
        }

        private void OnMcpToolChanged(MCPToolChangedEventArgs e)
        {
            var tools = e.Tools;
            ShowTools(tools ?? new List<MCPTool>());
        }

        private void UcAgentDef_Load(object? sender, EventArgs e)
        {
            InitContentConfig();
            var tools = Context.Instance.MCPTools;
            ShowTools(tools ?? new List<MCPTool>());
            if (agent != null)
            {
                ShowAgent(agent);
            }
        }

        private void ShowTools(List<MCPTool> tools)
        {
            if (tools == null || tools.Count == 0) return;

            var table = tools.ToDataTable();
            table.Columns.Add("Id", typeof(long));
            table.Columns.Add("checked", typeof(bool));
            table.BeginInit();
            foreach (DataRow row in table.Rows)
            {
                row["Id"] = 0L;
                row["checked"] = false;
            }

            table.EndInit();
            table.AcceptChanges();
            dgvTools.DataSource = table;


            if (agent != null)
            {
                var agentTools = agent.Tools;
                if (agentTools != null)
                {

                    foreach (var tool in agentTools)
                    {
                        var rows = table.Select($"name='{tool.Name}'");
                        if (rows == null || rows.Length == 0) continue;
                        foreach (DataRow row in rows)
                        {
                            row.BeginEdit();
                            row["checked"] = true;
                            row["Id"] = tool.Id;
                            row.EndEdit();
                        }
                    }
                }

            }
        }

        private void ShowSkills()
        {
            if (agent.Skills == null) return;
            foreach (var skill in agent.Skills)
            {
                var uc = new ucSkill
                {
                    Dock = DockStyle.Fill,
                    Name = "skill"
                };
                uc.NameChanged += Uc_NameChanged;
                var tabPage = new TabPage { Text = skill.Name };
                uc.SkillName = skill.Name;

                uc.SkillDescription = skill.SkillPrompt;
                tabPage.Tag = skill;

                tabPage.Controls.Add(uc);
                tabSkills.Controls.Add(tabPage);
                tabSkills.SelectedTab = tabPage;
            }
        }


        private void OnNewSkill(object sender, EventArgs e)
        {
            var uc = new ucSkill
            {
                Dock = DockStyle.Fill,
                Name = "skill"
            };
            uc.NameChanged += Uc_NameChanged;
            var tabPage = new TabPage
            {
                Text = "新的技能"
            };
            uc.SkillName = "新的技能";
            tabPage.Controls.Add(uc);
            tabSkills.Controls.Add(tabPage);
            tabSkills.SelectedTab = tabPage;
        }

        private void Uc_NameChanged(object? sender, KeyEventArgs e)
        {
            var txtName = sender as TextBox;
            if (txtName != null)
            {
                var tab = tabSkills.SelectedTab;
                if (tab == null) return;
                tab.Text = txtName.Text;
            }
        }

        private void OnRemoveSkill(object sender, EventArgs e)
        {
            var tab = tabSkills.SelectedTab;
            if (tab == null) return;
            tabSkills.Controls.Remove(tab);
        }

        private void OnAgentConfirm(object sender, EventArgs e)
        {
            var datastore = new DataStore();
            //save agent
            var newAgent = new AgentInfo
            {
                Id = DateTime.Now.Ticks,
                Name = txtName.Text,
                Defination = txtDef.Text,
                State = ABL.Object.EnumEntityState.Added
            };

            if (agent != null)
            {
                newAgent.Id = agent.Id;
                newAgent.State = ABL.Object.EnumEntityState.Modified;
            }

            datastore.Save(newAgent);

            //save tools
            List<AgentMcpToolInfo> rawTools;
            if (agent != null && agent.Tools != null) rawTools = agent.Tools;
            else rawTools = new List<AgentMcpToolInfo>();

            var toolSet = rawTools.ToDictionary(x => x.Name, y => y);
            var newTools = new List<AgentMcpToolInfo>();
            var ticks = DateTime.Now.Ticks;
            if (dgvTools.DataSource is DataTable table)
            {
                if (table != null)
                {
                    var rows = table.Select("checked=1");
                    var newt = table.Clone();
                    foreach (var row in rows)
                    {
                        newt.Rows.Add(row.ItemArray);
                    }
                    newt.AcceptChanges();

                    newTools = newt.ConvertList<AgentMcpToolInfo>((t) =>
                    {
                        t.AgentId = newAgent.Id;
                        if (t.Id == 0)
                        {
                            t.Id = ticks;
                            ticks++;
                        }
                    });
                }
            }

            var mcpCompare = new AgentMcpDataStateChecker();
            var changedMcps = mcpCompare.Compare(rawTools, newTools);
            if (changedMcps != null)
            {
                datastore.Save(changedMcps);
            }


            //save skills info
            List<AgentSkillInfo> rawSkills = null;
            if (agent == null || agent.Skills == null) rawSkills = new List<AgentSkillInfo>();
            else rawSkills = agent.Skills;

            var newSkills = new List<AgentSkillInfo>();
            foreach (TabPage tab in tabSkills.TabPages)
            {
                var ucs = tab.Controls.Find("skill", false);
                if (ucs == null) continue;

                if (ucs == null || ucs.Length == 0) continue;
                var uc = ucs[0];
                var oldSkill = tab.Tag as AgentSkillInfo;
                if (uc is ucSkill skill)
                {
                    var newSkill = new AgentSkillInfo
                    {
                        Id = oldSkill == null ? DateTime.Now.Ticks : oldSkill.Id,
                        Name = skill.SkillName,
                        AgentId = newAgent.Id,
                        SkillPrompt = skill.SkillDescription
                    };

                    newSkills.Add(newSkill);
                }
            }

            var skillCompare = new AgentSkillDataStateChecker();
            var changedSkills = skillCompare.Compare(rawSkills, newSkills);
            if (changedSkills != null)
            {
                datastore.Save(changedSkills);
            }

            //save store content
            List<ContentStoreConfig> rawContentConfigs = null;
            if (agent == null || agent.Skills == null) rawContentConfigs = new List<ContentStoreConfig>();
            else rawContentConfigs = agent.ContentStores;

            var newContentConfigs = tvConfigs.Nodes.Cast<TreeNode>().Select(x =>
            {
                var config = x.Tag as ContentStoreConfig;
                config.AgentId = newAgent.Id;
                return config;
            }).ToList();

            var configCompare = new AgentContentStoreConfigStateChecker();
            var chagnedConfigs = configCompare.Compare(rawContentConfigs, newContentConfigs);
            if (chagnedConfigs != null)
            {
                datastore.Save(chagnedConfigs);
            }

            var c = Context.Instance.CurrentAgent;
            if (c.Id == newAgent.Id)
            {
                Context.Instance.CurrentAgent = newAgent;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


        private void InitContentConfig()
        {
            var contentTypes = EnumHelper.Helper.Items(typeof(ContentType));
            var storeTypes = EnumHelper.Helper.Items(typeof(StoreType));
            var dbTypes = EnumHelper.Helper.Items(typeof(EnumConfigDbType));
            this.BindItem(cmbContentType, contentTypes);
            this.BindItem(cmbStoreType, storeTypes);
            this.BindItem(cmbDbType, dbTypes);
        }

        private void ClearConfig()
        {
            txtConfigName.Tag = null;
            txtConfigName.Text = string.Empty;
            txtPrefix.Text = string.Empty;
            cmbContentType.SelectedIndex = 0;
            cmbStoreType.SelectedIndex = 0;
            txtDir.Text = string.Empty;
            cmbDbType.SelectedIndex = 0;
            txtDbConstr.Text = string.Empty;
            txtDBInsertSql.Text = string.Empty;
            txtWebApi.Text = string.Empty;
        }

        private void BindItem(ComboBox cmb, List<Item> items)
        {
            cmb.Items.Clear();
            cmb.DisplayMember = "Name";
            cmb.ValueMember = "Id";
            cmb.DataSource = items;
        }

        private void OnNewContentConfig(object sender, EventArgs e)
        {
            ClearConfig();
            txtConfigName.Tag = null;
        }

        private void OnDeleteContentConfig(object sender, EventArgs e)
        {
            ClearConfig();

            var node = tvConfigs.SelectedNode;
            if (node == null) return;
            tvConfigs.Nodes.Remove(node);
        }

        private void OnConfirmContentConfig(object sender, EventArgs e)
        {
            var data = txtConfigName.Tag as ContentStoreConfig;
            if (data == null) data = new ContentStoreConfig() { Id = DateTime.Now.Ticks };

            data.Name = txtConfigName.Text;
            data.Regex = txtPrefix.Text;
            data.ContentType = EnumHelper.Helper.ParseDesc<ContentType>(cmbContentType.SelectedValue.ToString());
            data.StoreType = EnumHelper.Helper.ParseDesc<StoreType>(cmbStoreType.SelectedValue.ToString());
            data.Dir = txtDir.Text;
            data.Db = EnumHelper.Helper.ParseDesc<EnumConfigDbType>(cmbDbType.SelectedValue.ToString());
            data.ConnectionString = txtDbConstr.Text;
            data.InsertSQL = txtDBInsertSql.Text;
            data.WebApi = txtWebApi.Text;

            foreach (TreeNode node in tvConfigs.Nodes)
            {
                var tag = node.Tag as ContentStoreConfig;
                if (tag == null) continue;

                if (tag.Id == data.Id)
                {
                    node.Tag = data;
                    return;
                }
            }

            var newNode = new TreeNode
            {
                Text = txtConfigName.Text,
                Tag = data
            };
            tvConfigs.Nodes.Add(newNode);
        }

        private void OnEditContentConfig(object sender, EventArgs e)
        {
            var data = tvConfigs.SelectedNode?.Tag as ContentStoreConfig;
            if (data == null) return;

            ClearConfig();

            txtConfigName.Text = data.Name;
            txtPrefix.Text = data.Regex;
            cmbContentType.SelectedValue = data.ContentType.ToString();
            cmbStoreType.SelectedValue = data.StoreType.ToString();
            txtDir.Text = data.Dir;
            cmbDbType.SelectedValue = data.Db.ToString();
            txtDbConstr.Text = data.ConnectionString;
            txtDBInsertSql.Text = data.InsertSQL;
            txtWebApi.Text = data.WebApi;
        }

        private class AgentMcpDataStateChecker : AbstractDataStateChecker<AgentMcpToolInfo, String>
        {
            protected override string GetKey(AgentMcpToolInfo o)
            {
                return o.Name;
            }
        }

        private class AgentSkillDataStateChecker : AbstractDataStateChecker<AgentSkillInfo, long>
        {
            protected override long GetKey(AgentSkillInfo o)
            {
                return o.Id;
            }
        }

        private class AgentContentStoreConfigStateChecker : AbstractDataStateChecker<ContentStoreConfig, long>
        {
            protected override long GetKey(ContentStoreConfig o)
            {
                return o.Id;
            }
        }

        private class McpToolExtend : AgentMcpToolInfo
        {
            [Field(IgnoreCase = true)]
            public bool Checked { get; set; }
        }
    }
}
