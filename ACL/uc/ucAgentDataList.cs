using ABL;
using ACL.dao;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ACL.uc
{
    public partial class ucAgentDataList : UserControl
    {
        public ucAgentDataList()
        {
            InitializeComponent();


            this.Load += UcAgentDataList_Load;
            dgvAgent.AutoGenerateColumns = false;
        }

        private void UcAgentDataList_Load(object? sender, EventArgs e)
        {
            LoadAgents();
        }

        private void LoadAgents()
        {
            dgvAgent.DataSource = null;
            var datastore = new DataStore();
            var agents = datastore.Fill<AgentInfo>();
            agents = agents ?? new List<AgentInfo>();
            var table = agents.ToDataTable();
            table.Columns.Remove("State");
            table.Columns.Remove("Defination");
            table.AcceptChanges();

            dgvAgent.DataSource = table;
        }

        private void OnAgentItemClick(object? sender, EventArgs e)
        {
            var agent = sender as AgentInfo;
            if (agent == null) return;

        }

        private void OnNewAgent(object sender, EventArgs e)
        {
            var uc = new ucAgentDef();
            uc.StartPosition = FormStartPosition.CenterParent;
            if (uc.ShowDialog() == DialogResult.OK)
            {
                LoadAgents();
            }
        }

        private void OnDeleteAgent(object sender, EventArgs e)
        {
            var rows = dgvAgent.SelectedRows;
            if (rows == null || rows.Count == 0) return;
            var id = (long?)rows[0].Cells[0].Value;
            if (id == null) return;

            var agent = new AgentInfo() { Id = id.Value, State = ABL.Object.EnumEntityState.Deleted };
            var store = new DataStore();
            store.Save(agent);
            LoadAgents();
        }


        private void OnEditAgent(object sender, EventArgs e)
        {
            var rows = dgvAgent.SelectedRows;
            if (rows == null || rows.Count == 0) return;
            var id = (long?)rows[0].Cells[0].Value;
            if (id == null) return;

            var store = new DataStore();
            var agent = store.GetAgent(id ?? 0);
            if (agent == null) return;

            var uc = new ucAgentDef();
            uc.StartPosition = FormStartPosition.CenterParent;
            uc.Agent = agent;
            if (uc.ShowDialog() == DialogResult.OK)
            {
                LoadAgents();
            }
        }
    }
}
