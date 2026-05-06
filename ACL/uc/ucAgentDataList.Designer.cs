namespace ACL.uc
{
    partial class ucAgentDataList
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucAgentDataList));
            panel2 = new Panel();
            dgvAgent = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            panel1 = new Panel();
            btnDeleteAgent = new Button();
            button1 = new Button();
            btnAddAgent = new Button();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAgent).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.Controls.Add(dgvAgent);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 46);
            panel2.Name = "panel2";
            panel2.Size = new Size(371, 608);
            panel2.TabIndex = 4;
            // 
            // dgvAgent
            // 
            dgvAgent.AllowUserToAddRows = false;
            dgvAgent.AllowUserToDeleteRows = false;
            dgvAgent.BackgroundColor = Color.White;
            dgvAgent.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAgent.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2 });
            dgvAgent.Dock = DockStyle.Fill;
            dgvAgent.Location = new Point(0, 0);
            dgvAgent.MultiSelect = false;
            dgvAgent.Name = "dgvAgent";
            dgvAgent.ReadOnly = true;
            dgvAgent.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAgent.ShowEditingIcon = false;
            dgvAgent.Size = new Size(371, 608);
            dgvAgent.TabIndex = 1;
            // 
            // Column1
            // 
            Column1.DataPropertyName = "Id";
            Column1.HeaderText = "编号";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            // 
            // Column2
            // 
            Column2.DataPropertyName = "Name";
            Column2.HeaderText = "名称";
            Column2.Name = "Column2";
            Column2.ReadOnly = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnDeleteAgent);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(btnAddAgent);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(371, 46);
            panel1.TabIndex = 3;
            // 
            // btnDeleteAgent
            // 
            btnDeleteAgent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            btnDeleteAgent.FlatAppearance.BorderSize = 0;
            btnDeleteAgent.FlatStyle = FlatStyle.Flat;
            btnDeleteAgent.Image = (Image)resources.GetObject("btnDeleteAgent.Image");
            btnDeleteAgent.Location = new Point(108, 2);
            btnDeleteAgent.Name = "btnDeleteAgent";
            btnDeleteAgent.Size = new Size(46, 40);
            btnDeleteAgent.TabIndex = 0;
            btnDeleteAgent.UseVisualStyleBackColor = true;
            btnDeleteAgent.Click += OnDeleteAgent;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Image = (Image)resources.GetObject("button1.Image");
            button1.Location = new Point(55, 2);
            button1.Name = "button1";
            button1.Size = new Size(46, 40);
            button1.TabIndex = 0;
            button1.UseVisualStyleBackColor = true;
            button1.Click += OnEditAgent;
            // 
            // btnAddAgent
            // 
            btnAddAgent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddAgent.FlatAppearance.BorderSize = 0;
            btnAddAgent.FlatStyle = FlatStyle.Flat;
            btnAddAgent.Image = (Image)resources.GetObject("btnAddAgent.Image");
            btnAddAgent.Location = new Point(3, 2);
            btnAddAgent.Name = "btnAddAgent";
            btnAddAgent.Size = new Size(46, 40);
            btnAddAgent.TabIndex = 0;
            btnAddAgent.UseVisualStyleBackColor = true;
            btnAddAgent.Click += OnNewAgent;
            // 
            // ucAgentDataList
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "ucAgentDataList";
            Size = new Size(371, 654);
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvAgent).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel2;
        private DataGridView dgvAgent;
        private Panel panel1;
        private Button btnDeleteAgent;
        private Button btnAddAgent;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private Button button1;
    }
}
