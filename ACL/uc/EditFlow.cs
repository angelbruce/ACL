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
    public partial class EditFlow : Form
    {
        private FlowInfo flow = null;
        public EditFlow()
        {
            InitializeComponent();
            this.Load += NewFlow_Load;
        }

        private void NewFlow_Load(object? sender, EventArgs e)
        {
            if (flow != null) ShowFlow();
        }

        private void ShowFlow()
        {
            txtName.Text = flow.Name;
            txtDef.Text = flow.Description;

        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FlowInfo Flow
        {
            get { return this.flow; }
            set { this.flow = value; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            var datastore = new DataStore();
            var saveFlow = new FlowInfo();
            saveFlow.State = flow == null ? ABL.Object.EnumEntityState.Added : ABL.Object.EnumEntityState.Modified;
            saveFlow.Id = flow == null ? 0 : flow.Id;
            saveFlow.Name = txtName.Text;
            saveFlow.Description = txtDef.Text;

            datastore.Save(saveFlow);

            this.Close();
        }


    }
}
