using ABL;
using ACL.business.flow;
using ACL.dao;
using System.ComponentModel;
using System.Threading.Channels;

namespace ACL.uc
{
    public partial class ucFlowDataList : UserControl
    {
        private FlowAgent agent;
        private Channel<string> output;
        private Channel<string> input;
        private Panel panelContainer;
        private Panel panelOutput;
        private RichTextBox txtOutput;
        public ucFlowDataList()
        {
            InitializeComponent();
            this.Load += UcFlow_Load;
            dgvFlow.AutoGenerateColumns = false;
            output = Channel.CreateUnbounded<string>();
            input = Channel.CreateUnbounded<string>();
        }

        private void UcFlow_Load(object? sender, EventArgs e)
        {
            txtOutput = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            PanelOutput.Controls.Add(txtOutput);
            ShowFlows();
            Task.Run(async () =>
            {
                while (true)
                {
                    var msg = await output.Reader.ReadAsync();
                    this.Invoke(() =>
                    {
                        txtOutput.AppendText(msg);
                        if(msg.Contains('\n'))
                        {
                            txtOutput.ScrollToCaret();
                        }
                    });
                }
            });
            panelOutput.Invalidate();
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Panel PanelContainer
        {
            get { return panelContainer; }
            set { panelContainer = value; }
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Panel PanelOutput
        {
            get { return panelOutput; }
            set { panelOutput = value; }
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
                txtOutput.Clear();
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



                agent = new FlowAgent(id, output, input);
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

        private void OnSendAsk(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Control)
            {
                var inputData = txtIput.Text.Trim();
                if (!string.IsNullOrEmpty(inputData))
                {
                    input.Writer.WriteAsync(inputData);
                    output.Writer.WriteAsync(inputData);
                    txtIput.Clear();
                }
            }
        }
    }
}
