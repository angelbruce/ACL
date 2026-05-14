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
            button4 = new Button();
            button3 = new Button();
            btnDeleteAgent = new Button();
            button2 = new Button();
            btnAddAgent = new Button();
            button1 = new Button();
            panel2 = new Panel();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            dgvFlow = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            tabPage2 = new TabPage();
            txtIput = new TextBox();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvFlow).BeginInit();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(button4);
            panel1.Controls.Add(button3);
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
            // button4
            // 
            button4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            button4.FlatAppearance.BorderSize = 0;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Image = (Image)resources.GetObject("button4.Image");
            button4.Location = new Point(130, 9);
            button4.Name = "button4";
            button4.Size = new Size(20, 32);
            button4.TabIndex = 4;
            button4.UseVisualStyleBackColor = true;
            button4.Click += OnStop;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Image = (Image)resources.GetObject("button3.Image");
            button3.Location = new Point(104, 9);
            button3.Name = "button3";
            button3.Size = new Size(20, 32);
            button3.TabIndex = 4;
            button3.UseVisualStyleBackColor = true;
            button3.Click += OnStart;
            // 
            // btnDeleteAgent
            // 
            btnDeleteAgent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            btnDeleteAgent.FlatAppearance.BorderSize = 0;
            btnDeleteAgent.FlatStyle = FlatStyle.Flat;
            btnDeleteAgent.Image = (Image)resources.GetObject("btnDeleteAgent.Image");
            btnDeleteAgent.Location = new Point(54, 7);
            btnDeleteAgent.Name = "btnDeleteAgent";
            btnDeleteAgent.Size = new Size(20, 32);
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
            button2.Location = new Point(28, 7);
            button2.Name = "button2";
            button2.Size = new Size(20, 32);
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
            btnAddAgent.Size = new Size(20, 32);
            btnAddAgent.TabIndex = 6;
            btnAddAgent.UseVisualStyleBackColor = true;
            btnAddAgent.Click += btnAdd_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            button1.Location = new Point(483, 13);
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
            panel2.BackColor = SystemColors.Control;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(tabControl1);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 46);
            panel2.Name = "panel2";
            panel2.Size = new Size(555, 626);
            panel2.TabIndex = 1;
            // 
            // tabControl1
            // 
            tabControl1.Alignment = TabAlignment.Bottom;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(553, 624);
            tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(dgvFlow);
            tabPage1.Location = new Point(4, 4);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(545, 594);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "流程列表";
            tabPage1.UseVisualStyleBackColor = true;
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
            dgvFlow.Location = new Point(3, 3);
            dgvFlow.MultiSelect = false;
            dgvFlow.Name = "dgvFlow";
            dgvFlow.ReadOnly = true;
            dgvFlow.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFlow.Size = new Size(539, 588);
            dgvFlow.TabIndex = 3;
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
            // tabPage2
            // 
            tabPage2.Controls.Add(txtIput);
            tabPage2.Location = new Point(4, 4);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(545, 594);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "流程输入";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtIput
            // 
            txtIput.Dock = DockStyle.Fill;
            txtIput.Location = new Point(3, 3);
            txtIput.Multiline = true;
            txtIput.Name = "txtIput";
            txtIput.Size = new Size(539, 588);
            txtIput.TabIndex = 6;
            txtIput.KeyUp += OnSendAsk;
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
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvFlow).EndInit();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Button button1;
        private Button btnDeleteAgent;
        private Button button2;
        private Button btnAddAgent;
        private Button button3;
        private Button button4;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private DataGridView dgvFlow;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private TabPage tabPage2;
        private TextBox txtIput;
    }
}
