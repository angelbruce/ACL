namespace ACL.uc
{
    partial class ucFlowDataList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucFlowDataList));
            panel1 = new Panel();
            button1 = new Button();
            panel2 = new Panel();
            dgvFlow = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            btnDeleteAgent = new Button();
            button2 = new Button();
            btnAddAgent = new Button();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvFlow).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(btnDeleteAgent);
            panel1.Controls.Add(button2);
            panel1.Controls.Add(btnAddAgent);
            panel1.Controls.Add(button1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(555, 46);
            panel1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            button1.Location = new Point(174, 7);
            button1.Name = "button1";
            button1.Size = new Size(48, 33);
            button1.TabIndex = 3;
            button1.Text = "配置";
            button1.UseVisualStyleBackColor = true;
            button1.Visible = false;
            button1.Click += OnConfig;
            // 
            // panel2
            // 
            panel2.BackColor = Color.Gray;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(dgvFlow);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 46);
            panel2.Name = "panel2";
            panel2.Size = new Size(555, 626);
            panel2.TabIndex = 1;
            // 
            // dgvFlow
            // 
            dgvFlow.AllowUserToAddRows = false;
            dgvFlow.AllowUserToDeleteRows = false;
            dgvFlow.BackgroundColor = Color.White;
            dgvFlow.BorderStyle = BorderStyle.None;
            dgvFlow.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvFlow.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2 });
            dgvFlow.Dock = DockStyle.Fill;
            dgvFlow.GridColor = Color.Gray;
            dgvFlow.Location = new Point(0, 0);
            dgvFlow.MultiSelect = false;
            dgvFlow.Name = "dgvFlow";
            dgvFlow.ReadOnly = true;
            dgvFlow.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFlow.Size = new Size(553, 624);
            dgvFlow.TabIndex = 2;
            dgvFlow.CellClick += dgvFlow_CellClick;
            // 
            // Column1
            // 
            Column1.DataPropertyName = "Id";
            Column1.HeaderText = "编号";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            Column1.Visible = false;
            // 
            // Column2
            // 
            Column2.DataPropertyName = "Name";
            Column2.HeaderText = "名称";
            Column2.Name = "Column2";
            Column2.ReadOnly = true;
            // 
            // btnDeleteAgent
            // 
            btnDeleteAgent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            btnDeleteAgent.FlatAppearance.BorderSize = 0;
            btnDeleteAgent.FlatStyle = FlatStyle.Flat;
            btnDeleteAgent.Image = (Image)resources.GetObject("btnDeleteAgent.Image");
            btnDeleteAgent.Location = new Point(108, 7);
            btnDeleteAgent.Name = "btnDeleteAgent";
            btnDeleteAgent.Size = new Size(46, 32);
            btnDeleteAgent.TabIndex = 4;
            btnDeleteAgent.UseVisualStyleBackColor = true;
            btnDeleteAgent.Click += btnDelete_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Image = (Image)resources.GetObject("button2.Image");
            button2.Location = new Point(55, 7);
            button2.Name = "button2";
            button2.Size = new Size(46, 32);
            button2.TabIndex = 5;
            button2.UseVisualStyleBackColor = true;
            button2.Click += btnEdit_Click;
            // 
            // btnAddAgent
            // 
            btnAddAgent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddAgent.FlatAppearance.BorderSize = 0;
            btnAddAgent.FlatStyle = FlatStyle.Flat;
            btnAddAgent.Image = (Image)resources.GetObject("btnAddAgent.Image");
            btnAddAgent.Location = new Point(3, 7);
            btnAddAgent.Name = "btnAddAgent";
            btnAddAgent.Size = new Size(46, 32);
            btnAddAgent.TabIndex = 6;
            btnAddAgent.UseVisualStyleBackColor = true;
            btnAddAgent.Click += btnAdd_Click;
            // 
            // ucFlowDataList
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "ucFlowDataList";
            Size = new Size(555, 672);
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvFlow).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private DataGridView dgvFlow;
        private Button button1;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private Button btnDeleteAgent;
        private Button button2;
        private Button btnAddAgent;
    }
}
