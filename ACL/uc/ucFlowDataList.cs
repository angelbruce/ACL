using ABL;
using ACL.business.flow;
using ACL.dao;
using System.ComponentModel;

namespace ACL.uc
{
    public partial class ucFlowDataList : UserControl
    {
        private FlowAgent agent;
        private Panel panelContainer;
        public ucFlowDataList()
        {
            InitializeComponent();
            this.Load += UcFlow_Load;
            dgvFlow.AutoGenerateColumns = false;
        }

        private void UcFlow_Load(object? sender, EventArgs e)
        {
            ShowFlows();
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Panel PanelContainer
        {
            get { return panelContainer; }
            set { panelContainer = value; }
        }

        private void ShowFlows()
        {
            var datastore = new DataStore();
            var flows = datastore.Fill<FlowInfo>();
            flows = flows ?? new List<FlowInfo>();
            var table = flows.ToDataTable();
            table.AcceptChanges();
            this.dgvFlow.DataSource = table;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var frm = new EditFlow();
            if (frm.ShowDialog() != DialogResult.OK) return;

            ShowFlows();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var rows = this.dgvFlow.SelectedRows;
            if (rows == null || rows.Count == 0) return;

            var id = (long)rows[0].Cells[0].Value;
            var datastore = new DataStore();
            var flow = datastore.GetFlow(id);
            if (flow == null) return;

            var frm = new EditFlow();
            frm.Flow = flow;

            if (frm.ShowDialog() != DialogResult.OK) return;

            ShowFlows();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var rows = this.dgvFlow.SelectedRows;
            if (rows == null || rows.Count == 0) return;

            var id = (long)rows[0].Cells[0].Value;

            var datastore = new DataStore();
            var flow = datastore.GetFlow(id);
            if (flow == null) return;

            var flowInfo = new FlowInfo();
            flowInfo.Id = id;
            flowInfo.State = ABL.Object.EnumEntityState.Deleted;
            datastore.Save(flowInfo);

            ShowFlows();
        }

        private void OnConfig(object sender, EventArgs e)
        {
            panelContainer.Controls.Clear();
            var rows = this.dgvFlow.SelectedRows;
            if (rows == null || rows.Count == 0) return;

            var id = (long)rows[0].Cells[0].Value;

            var datastore = new DataStore();
            var flow = datastore.GetFlow(id);
            if (flow == null) return;

            var closeTab = new ClosedPageTab();
            closeTab.FlowId = id.ToString();
            closeTab.Tag = flow;
            closeTab.Dock = DockStyle.Fill;
            panelContainer.Controls.Add(closeTab);
        }

        private void dgvFlow_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            OnConfig(sender, e);
        }

        private void OnStart(object sender, EventArgs e)
        {
            try
            {
                var rows = this.dgvFlow.SelectedRows;
                if (rows == null || rows.Count == 0) return;

                var id = (long)rows[0].Cells[0].Value;
                if (agent != null)
                {
                    if (id == agent.FlowId)
                    {
                        agent.Configure();
                        agent.Start();
                        return;
                    }
                }



                agent = new FlowAgent(id, null);
                agent.Configure();
                agent.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnStop(object sender, EventArgs e)
        {
            var rows = this.dgvFlow.SelectedRows;
            if (rows == null || rows.Count == 0) return;

            var id = (long)rows[0].Cells[0].Value;
            if (agent != null)
            {
                if (id == agent.FlowId)
                {
                    agent.Stop();
                    return;
                }
            }
        }
    }
}
