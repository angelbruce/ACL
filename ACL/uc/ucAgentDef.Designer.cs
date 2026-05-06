namespace ACL.uc
{
    partial class ucAgentDef
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucAgentDef));
            panel1 = new Panel();
            button2 = new Button();
            button1 = new Button();
            panel2 = new Panel();
            tab = new TabControl();
            tabPage1 = new TabPage();
            txtDef = new RichTextBox();
            txtName = new TextBox();
            label2 = new Label();
            label1 = new Label();
            tabPage2 = new TabPage();
            panel4 = new Panel();
            dgvTools = new DataGridView();
            Column1 = new DataGridViewCheckBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            Column5 = new DataGridViewTextBoxColumn();
            panel3 = new Panel();
            label3 = new Label();
            tabPage3 = new TabPage();
            panel6 = new Panel();
            tabSkills = new TabControl();
            panel5 = new Panel();
            btnRemove = new Button();
            btnAdd = new Button();
            tabStore = new TabPage();
            splitContainer1 = new SplitContainer();
            tvConfigs = new TreeView();
            toolStrip2 = new ToolStrip();
            toolStripButton1 = new ToolStripButton();
            toolStripButton3 = new ToolStripButton();
            toolStripButton2 = new ToolStripButton();
            toolStrip1 = new ToolStrip();
            tsbEditConfig = new ToolStripButton();
            cmbDbType = new ComboBox();
            cmbStoreType = new ComboBox();
            label12 = new Label();
            label11 = new Label();
            label10 = new Label();
            label8 = new Label();
            label9 = new Label();
            cmbContentType = new ComboBox();
            label7 = new Label();
            txtWebApi = new TextBox();
            txtDBInsertSql = new TextBox();
            txtDbConstr = new TextBox();
            txtDir = new TextBox();
            txtPrefix = new TextBox();
            label6 = new Label();
            label5 = new Label();
            txtConfigName = new TextBox();
            label4 = new Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            tab.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTools).BeginInit();
            panel3.SuspendLayout();
            tabPage3.SuspendLayout();
            panel6.SuspendLayout();
            panel5.SuspendLayout();
            tabStore.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            toolStrip2.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(button2);
            panel1.Controls.Add(button1);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 509);
            panel1.Name = "panel1";
            panel1.Size = new Size(799, 44);
            panel1.TabIndex = 0;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            button2.Image = (Image)resources.GetObject("button2.Image");
            button2.Location = new Point(582, 9);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 1;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            button1.Image = (Image)resources.GetObject("button1.Image");
            button1.Location = new Point(663, 9);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.UseVisualStyleBackColor = true;
            button1.Click += OnAgentConfirm;
            // 
            // panel2
            // 
            panel2.Controls.Add(tab);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(799, 509);
            panel2.TabIndex = 1;
            // 
            // tab
            // 
            tab.Controls.Add(tabPage1);
            tab.Controls.Add(tabPage2);
            tab.Controls.Add(tabPage3);
            tab.Controls.Add(tabStore);
            tab.Dock = DockStyle.Fill;
            tab.Location = new Point(0, 0);
            tab.Name = "tab";
            tab.SelectedIndex = 0;
            tab.Size = new Size(799, 509);
            tab.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(txtDef);
            tabPage1.Controls.Add(txtName);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(label1);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(791, 479);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "基本信息";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // txtDef
            // 
            txtDef.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtDef.Location = new Point(82, 74);
            txtDef.Name = "txtDef";
            txtDef.Size = new Size(686, 363);
            txtDef.TabIndex = 3;
            txtDef.Text = "";
            // 
            // txtName
            // 
            txtName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtName.Location = new Point(84, 45);
            txtName.Name = "txtName";
            txtName.Size = new Size(686, 23);
            txtName.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(46, 77);
            label2.Name = "label2";
            label2.Size = new Size(32, 17);
            label2.TabIndex = 1;
            label2.Text = "定义";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(46, 49);
            label1.Name = "label1";
            label1.Size = new Size(32, 17);
            label1.TabIndex = 0;
            label1.Text = "名称";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(panel4);
            tabPage2.Controls.Add(panel3);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(791, 479);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "可用工具";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            panel4.Controls.Add(dgvTools);
            panel4.Dock = DockStyle.Fill;
            panel4.Location = new Point(3, 38);
            panel4.Name = "panel4";
            panel4.Size = new Size(785, 438);
            panel4.TabIndex = 2;
            // 
            // dgvTools
            // 
            dgvTools.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvTools.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dgvTools.BackgroundColor = Color.White;
            dgvTools.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTools.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2, Column3, Column4, Column5 });
            dgvTools.Dock = DockStyle.Fill;
            dgvTools.Location = new Point(0, 0);
            dgvTools.Name = "dgvTools";
            dgvTools.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTools.Size = new Size(785, 438);
            dgvTools.TabIndex = 1;
            // 
            // Column1
            // 
            Column1.DataPropertyName = "checked";
            Column1.HeaderText = "选项";
            Column1.Name = "Column1";
            Column1.TrueValue = "true";
            Column1.Width = 38;
            // 
            // Column2
            // 
            Column2.DataPropertyName = "Name";
            Column2.HeaderText = "工具名称";
            Column2.Name = "Column2";
            Column2.ReadOnly = true;
            Column2.Width = 81;
            // 
            // Column3
            // 
            Column3.DataPropertyName = "Description";
            Column3.HeaderText = "工具说明";
            Column3.Name = "Column3";
            Column3.ReadOnly = true;
            Column3.Width = 81;
            // 
            // Column4
            // 
            Column4.DataPropertyName = "InputSchema";
            Column4.HeaderText = "输入参数";
            Column4.Name = "Column4";
            Column4.ReadOnly = true;
            Column4.Width = 81;
            // 
            // Column5
            // 
            Column5.DataPropertyName = "OutputSchema";
            Column5.HeaderText = "输出参数";
            Column5.Name = "Column5";
            Column5.ReadOnly = true;
            Column5.Width = 81;
            // 
            // panel3
            // 
            panel3.Controls.Add(label3);
            panel3.Dock = DockStyle.Top;
            panel3.Location = new Point(3, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(785, 35);
            panel3.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(15, 9);
            label3.Name = "label3";
            label3.Size = new Size(56, 17);
            label3.TabIndex = 0;
            label3.Text = "可用工具";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(panel6);
            tabPage3.Controls.Add(panel5);
            tabPage3.Location = new Point(4, 26);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(791, 479);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "技能定义";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel6
            // 
            panel6.Controls.Add(tabSkills);
            panel6.Dock = DockStyle.Fill;
            panel6.Location = new Point(3, 39);
            panel6.Name = "panel6";
            panel6.Size = new Size(785, 437);
            panel6.TabIndex = 1;
            // 
            // tabSkills
            // 
            tabSkills.Dock = DockStyle.Fill;
            tabSkills.Location = new Point(0, 0);
            tabSkills.Name = "tabSkills";
            tabSkills.SelectedIndex = 0;
            tabSkills.Size = new Size(785, 437);
            tabSkills.TabIndex = 2;
            // 
            // panel5
            // 
            panel5.Controls.Add(btnRemove);
            panel5.Controls.Add(btnAdd);
            panel5.Dock = DockStyle.Top;
            panel5.Location = new Point(3, 3);
            panel5.Name = "panel5";
            panel5.Size = new Size(785, 36);
            panel5.TabIndex = 0;
            // 
            // btnRemove
            // 
            btnRemove.Location = new Point(86, 7);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(75, 23);
            btnRemove.TabIndex = 0;
            btnRemove.Text = "删除";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += OnRemoveSkill;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(5, 7);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 23);
            btnAdd.TabIndex = 0;
            btnAdd.Text = "添加";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += OnNewSkill;
            // 
            // tabStore
            // 
            tabStore.Controls.Add(splitContainer1);
            tabStore.Location = new Point(4, 26);
            tabStore.Name = "tabStore";
            tabStore.Padding = new Padding(3);
            tabStore.Size = new Size(791, 479);
            tabStore.TabIndex = 3;
            tabStore.Text = "存储设定";
            tabStore.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.Location = new Point(3, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tvConfigs);
            splitContainer1.Panel1.Controls.Add(toolStrip2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(toolStrip1);
            splitContainer1.Panel2.Controls.Add(cmbDbType);
            splitContainer1.Panel2.Controls.Add(cmbStoreType);
            splitContainer1.Panel2.Controls.Add(label12);
            splitContainer1.Panel2.Controls.Add(label11);
            splitContainer1.Panel2.Controls.Add(label10);
            splitContainer1.Panel2.Controls.Add(label8);
            splitContainer1.Panel2.Controls.Add(label9);
            splitContainer1.Panel2.Controls.Add(cmbContentType);
            splitContainer1.Panel2.Controls.Add(label7);
            splitContainer1.Panel2.Controls.Add(txtWebApi);
            splitContainer1.Panel2.Controls.Add(txtDBInsertSql);
            splitContainer1.Panel2.Controls.Add(txtDbConstr);
            splitContainer1.Panel2.Controls.Add(txtDir);
            splitContainer1.Panel2.Controls.Add(txtPrefix);
            splitContainer1.Panel2.Controls.Add(label6);
            splitContainer1.Panel2.Controls.Add(label5);
            splitContainer1.Panel2.Controls.Add(txtConfigName);
            splitContainer1.Panel2.Controls.Add(label4);
            splitContainer1.Size = new Size(785, 473);
            splitContainer1.SplitterDistance = 215;
            splitContainer1.TabIndex = 0;
            // 
            // tvConfigs
            // 
            tvConfigs.Dock = DockStyle.Fill;
            tvConfigs.Location = new Point(0, 25);
            tvConfigs.Name = "tvConfigs";
            tvConfigs.Size = new Size(215, 448);
            tvConfigs.TabIndex = 1;
            // 
            // toolStrip2
            // 
            toolStrip2.Items.AddRange(new ToolStripItem[] { toolStripButton1, toolStripButton3, toolStripButton2 });
            toolStrip2.Location = new Point(0, 0);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.Size = new Size(215, 25);
            toolStrip2.TabIndex = 0;
            // 
            // toolStripButton1
            // 
            toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
            toolStripButton1.ImageTransparentColor = Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new Size(23, 22);
            toolStripButton1.Text = "添加配置";
            toolStripButton1.Click += OnNewContentConfig;
            // 
            // toolStripButton3
            // 
            toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton3.Image = (Image)resources.GetObject("toolStripButton3.Image");
            toolStripButton3.ImageTransparentColor = Color.Magenta;
            toolStripButton3.Name = "toolStripButton3";
            toolStripButton3.Size = new Size(23, 22);
            toolStripButton3.Text = "修改配置";
            toolStripButton3.Click += OnEditContentConfig;
            // 
            // toolStripButton2
            // 
            toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton2.Image = (Image)resources.GetObject("toolStripButton2.Image");
            toolStripButton2.ImageTransparentColor = Color.Magenta;
            toolStripButton2.Name = "toolStripButton2";
            toolStripButton2.Size = new Size(23, 22);
            toolStripButton2.Text = "删除配置";
            toolStripButton2.Click += OnDeleteContentConfig;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsbEditConfig });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(566, 25);
            toolStrip1.TabIndex = 3;
            toolStrip1.Text = "toolStrip1";
            // 
            // tsbEditConfig
            // 
            tsbEditConfig.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbEditConfig.Image = (Image)resources.GetObject("tsbEditConfig.Image");
            tsbEditConfig.ImageTransparentColor = Color.Magenta;
            tsbEditConfig.Name = "tsbEditConfig";
            tsbEditConfig.Size = new Size(23, 22);
            tsbEditConfig.Text = "确认";
            tsbEditConfig.Click += OnConfirmContentConfig;
            // 
            // cmbDbType
            // 
            cmbDbType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbDbType.FormattingEnabled = true;
            cmbDbType.Location = new Point(143, 223);
            cmbDbType.Name = "cmbDbType";
            cmbDbType.Size = new Size(369, 25);
            cmbDbType.TabIndex = 6;
            // 
            // cmbStoreType
            // 
            cmbStoreType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbStoreType.FormattingEnabled = true;
            cmbStoreType.Location = new Point(143, 163);
            cmbStoreType.Name = "cmbStoreType";
            cmbStoreType.Size = new Size(369, 25);
            cmbStoreType.TabIndex = 4;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(58, 315);
            label12.Name = "label12";
            label12.Size = new Size(78, 17);
            label12.TabIndex = 0;
            label12.Text = "WebApi地址";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(80, 286);
            label11.Name = "label11";
            label11.Size = new Size(56, 17);
            label11.TabIndex = 0;
            label11.Text = "插入语句";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(92, 257);
            label10.Name = "label10";
            label10.Size = new Size(44, 17);
            label10.TabIndex = 0;
            label10.Text = "连接串";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(80, 197);
            label8.Name = "label8";
            label8.Size = new Size(56, 17);
            label8.TabIndex = 0;
            label8.Text = "存储目录";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(68, 227);
            label9.Name = "label9";
            label9.Size = new Size(68, 17);
            label9.TabIndex = 0;
            label9.Text = "数据库类型";
            // 
            // cmbContentType
            // 
            cmbContentType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbContentType.FormattingEnabled = true;
            cmbContentType.Location = new Point(143, 132);
            cmbContentType.Name = "cmbContentType";
            cmbContentType.Size = new Size(369, 25);
            cmbContentType.TabIndex = 3;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(80, 167);
            label7.Name = "label7";
            label7.Size = new Size(56, 17);
            label7.TabIndex = 0;
            label7.Text = "存储类型";
            // 
            // txtWebApi
            // 
            txtWebApi.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtWebApi.Location = new Point(143, 312);
            txtWebApi.Name = "txtWebApi";
            txtWebApi.Size = new Size(369, 23);
            txtWebApi.TabIndex = 9;
            // 
            // txtDBInsertSql
            // 
            txtDBInsertSql.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDBInsertSql.Location = new Point(143, 283);
            txtDBInsertSql.Name = "txtDBInsertSql";
            txtDBInsertSql.Size = new Size(369, 23);
            txtDBInsertSql.TabIndex = 8;
            // 
            // txtDbConstr
            // 
            txtDbConstr.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDbConstr.Location = new Point(143, 254);
            txtDbConstr.Name = "txtDbConstr";
            txtDbConstr.Size = new Size(369, 23);
            txtDbConstr.TabIndex = 7;
            // 
            // txtDir
            // 
            txtDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDir.Location = new Point(143, 194);
            txtDir.Name = "txtDir";
            txtDir.Size = new Size(369, 23);
            txtDir.TabIndex = 5;
            // 
            // txtPrefix
            // 
            txtPrefix.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPrefix.Location = new Point(143, 103);
            txtPrefix.Name = "txtPrefix";
            txtPrefix.Size = new Size(369, 23);
            txtPrefix.TabIndex = 2;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(80, 136);
            label6.Name = "label6";
            label6.Size = new Size(56, 17);
            label6.TabIndex = 0;
            label6.Text = "内容类型";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(104, 106);
            label5.Name = "label5";
            label5.Size = new Size(32, 17);
            label5.TabIndex = 0;
            label5.Text = "前缀";
            // 
            // txtConfigName
            // 
            txtConfigName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtConfigName.Location = new Point(143, 74);
            txtConfigName.Name = "txtConfigName";
            txtConfigName.Size = new Size(369, 23);
            txtConfigName.TabIndex = 1;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(104, 77);
            label4.Name = "label4";
            label4.Size = new Size(32, 17);
            label4.TabIndex = 0;
            label4.Text = "名称";
            // 
            // ucAgentDef
            // 
            AcceptButton = button1;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = button2;
            ClientSize = new Size(799, 553);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Name = "ucAgentDef";
            Text = "Agent定义";
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            tab.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvTools).EndInit();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            tabPage3.ResumeLayout(false);
            panel6.ResumeLayout(false);
            panel5.ResumeLayout(false);
            tabStore.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            toolStrip2.ResumeLayout(false);
            toolStrip2.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button button2;
        private Button button1;
        private Panel panel2;
        private TabControl tab;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private RichTextBox txtDef;
        private TextBox txtName;
        private Label label2;
        private Label label1;
        private Panel panel4;
        private DataGridView dgvTools;
        private Panel panel3;
        private Label label3;
        private Panel panel6;
        private Panel panel5;
        private Button btnRemove;
        private Button btnAdd;
        private TabControl tabSkills;
        private DataGridViewCheckBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private DataGridViewTextBoxColumn Column5;
        private TabPage tabStore;
        private SplitContainer splitContainer1;
        private Label label4;
        private TextBox txtConfigName;
        private ComboBox cmbStoreType;
        private ComboBox cmbContentType;
        private Label label7;
        private TextBox txtPrefix;
        private Label label6;
        private Label label5;
        private Label label8;
        private TextBox txtDir;
        private ComboBox cmbDbType;
        private Label label9;
        private Label label11;
        private Label label10;
        private TextBox txtDBInsertSql;
        private TextBox txtDbConstr;
        private Label label12;
        private TextBox txtWebApi;
        private ToolStrip toolStrip1;
        private ToolStripButton tsbEditConfig;
        private TreeView tvConfigs;
        private ToolStrip toolStrip2;
        private ToolStripButton toolStripButton1;
        private ToolStripButton toolStripButton2;
        private ToolStripButton toolStripButton3;
    }
}