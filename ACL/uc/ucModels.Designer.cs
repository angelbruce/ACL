namespace ACL.uc
{
    partial class ucModels
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucModels));
            splitContainer1 = new SplitContainer();
            tvModels = new TreeView();
            panel1 = new Panel();
            btnDeleteAgent = new Button();
            button2 = new Button();
            btnAddAgent = new Button();
            chkDefault = new CheckBox();
            txtDescription = new TextBox();
            txtVersion = new TextBox();
            txtApiKey = new TextBox();
            txtAccessUrl = new TextBox();
            txtName = new TextBox();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            panel2 = new Panel();
            btnCancel = new Button();
            btnAccept = new Button();
            button1 = new Button();
            button3 = new Button();
            button4 = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tvModels);
            splitContainer1.Panel1.Controls.Add(panel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.BackColor = Color.White;
            splitContainer1.Panel2.Controls.Add(chkDefault);
            splitContainer1.Panel2.Controls.Add(txtDescription);
            splitContainer1.Panel2.Controls.Add(txtVersion);
            splitContainer1.Panel2.Controls.Add(txtApiKey);
            splitContainer1.Panel2.Controls.Add(txtAccessUrl);
            splitContainer1.Panel2.Controls.Add(txtName);
            splitContainer1.Panel2.Controls.Add(label6);
            splitContainer1.Panel2.Controls.Add(label5);
            splitContainer1.Panel2.Controls.Add(label4);
            splitContainer1.Panel2.Controls.Add(label3);
            splitContainer1.Panel2.Controls.Add(label2);
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Panel2.Controls.Add(panel2);
            splitContainer1.Size = new Size(852, 506);
            splitContainer1.SplitterDistance = 284;
            splitContainer1.TabIndex = 0;
            // 
            // tvModels
            // 
            tvModels.Dock = DockStyle.Fill;
            tvModels.Location = new Point(0, 31);
            tvModels.Name = "tvModels";
            tvModels.Size = new Size(284, 475);
            tvModels.TabIndex = 1;
            tvModels.AfterSelect += OnView;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnDeleteAgent);
            panel1.Controls.Add(button2);
            panel1.Controls.Add(btnAddAgent);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(284, 31);
            panel1.TabIndex = 0;
            // 
            // btnDeleteAgent
            // 
            btnDeleteAgent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            btnDeleteAgent.FlatAppearance.BorderSize = 0;
            btnDeleteAgent.FlatStyle = FlatStyle.Flat;
            btnDeleteAgent.Image = (Image)resources.GetObject("btnDeleteAgent.Image");
            btnDeleteAgent.Location = new Point(55, 5);
            btnDeleteAgent.Name = "btnDeleteAgent";
            btnDeleteAgent.Size = new Size(20, 20);
            btnDeleteAgent.TabIndex = 7;
            btnDeleteAgent.UseVisualStyleBackColor = true;
            btnDeleteAgent.Click += OnDelete;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Image = (Image)resources.GetObject("button2.Image");
            button2.Location = new Point(29, 5);
            button2.Name = "button2";
            button2.Size = new Size(20, 20);
            button2.TabIndex = 8;
            button2.UseVisualStyleBackColor = true;
            button2.Click += OnEdit;
            // 
            // btnAddAgent
            // 
            btnAddAgent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddAgent.FlatAppearance.BorderSize = 0;
            btnAddAgent.FlatStyle = FlatStyle.Flat;
            btnAddAgent.Image = (Image)resources.GetObject("btnAddAgent.Image");
            btnAddAgent.Location = new Point(4, 5);
            btnAddAgent.Name = "btnAddAgent";
            btnAddAgent.Size = new Size(20, 20);
            btnAddAgent.TabIndex = 9;
            btnAddAgent.UseVisualStyleBackColor = true;
            btnAddAgent.Click += OnAdd;
            // 
            // chkDefault
            // 
            chkDefault.AutoSize = true;
            chkDefault.Location = new Point(133, 383);
            chkDefault.Name = "chkDefault";
            chkDefault.Size = new Size(15, 14);
            chkDefault.TabIndex = 9;
            chkDefault.UseVisualStyleBackColor = true;
            // 
            // txtDescription
            // 
            txtDescription.Location = new Point(134, 266);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(382, 86);
            txtDescription.TabIndex = 8;
            // 
            // txtVersion
            // 
            txtVersion.Location = new Point(134, 217);
            txtVersion.Name = "txtVersion";
            txtVersion.Size = new Size(382, 23);
            txtVersion.TabIndex = 8;
            // 
            // txtApiKey
            // 
            txtApiKey.Location = new Point(134, 171);
            txtApiKey.Name = "txtApiKey";
            txtApiKey.Size = new Size(382, 23);
            txtApiKey.TabIndex = 8;
            // 
            // txtAccessUrl
            // 
            txtAccessUrl.Location = new Point(133, 125);
            txtAccessUrl.Name = "txtAccessUrl";
            txtAccessUrl.Size = new Size(382, 23);
            txtAccessUrl.TabIndex = 8;
            // 
            // txtName
            // 
            txtName.Location = new Point(134, 79);
            txtName.Name = "txtName";
            txtName.Size = new Size(382, 23);
            txtName.TabIndex = 8;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(71, 381);
            label6.Name = "label6";
            label6.Size = new Size(56, 17);
            label6.TabIndex = 7;
            label6.Text = "是否默认";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(95, 266);
            label5.Name = "label5";
            label5.Size = new Size(32, 17);
            label5.TabIndex = 6;
            label5.Text = "描述";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(83, 220);
            label4.Name = "label4";
            label4.Size = new Size(44, 17);
            label4.TabIndex = 5;
            label4.Text = "版本号";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(74, 174);
            label3.Name = "label3";
            label3.Size = new Size(53, 17);
            label3.TabIndex = 4;
            label3.Text = "API KEY";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(17, 128);
            label2.Name = "label2";
            label2.Size = new Size(110, 17);
            label2.TabIndex = 3;
            label2.Text = "访问地址(OPENAI)";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(71, 82);
            label1.Name = "label1";
            label1.Size = new Size(56, 17);
            label1.TabIndex = 2;
            label1.Text = "模型名称";
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.Control;
            panel2.Controls.Add(btnCancel);
            panel2.Controls.Add(btnAccept);
            panel2.Controls.Add(button1);
            panel2.Controls.Add(button3);
            panel2.Controls.Add(button4);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(564, 31);
            panel2.TabIndex = 1;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            btnCancel.Enabled = false;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Image = (Image)resources.GetObject("btnCancel.Image");
            btnCancel.Location = new Point(29, 5);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(20, 20);
            btnCancel.TabIndex = 11;
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnAccept
            // 
            btnAccept.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            btnAccept.Enabled = false;
            btnAccept.FlatAppearance.BorderSize = 0;
            btnAccept.FlatStyle = FlatStyle.Flat;
            btnAccept.Image = (Image)resources.GetObject("btnAccept.Image");
            btnAccept.Location = new Point(4, 5);
            btnAccept.Name = "btnAccept";
            btnAccept.Size = new Size(20, 20);
            btnAccept.TabIndex = 10;
            btnAccept.UseVisualStyleBackColor = true;
            btnAccept.Click += btnAccept_Click;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Image = (Image)resources.GetObject("button1.Image");
            button1.Location = new Point(55, 5);
            button1.Name = "button1";
            button1.Size = new Size(20, 0);
            button1.TabIndex = 7;
            button1.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Image = (Image)resources.GetObject("button3.Image");
            button3.Location = new Point(29, 5);
            button3.Name = "button3";
            button3.Size = new Size(20, 0);
            button3.TabIndex = 8;
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            button4.FlatAppearance.BorderSize = 0;
            button4.FlatStyle = FlatStyle.Flat;
            button4.Image = (Image)resources.GetObject("button4.Image");
            button4.Location = new Point(4, 5);
            button4.Name = "button4";
            button4.Size = new Size(20, 0);
            button4.TabIndex = 9;
            button4.UseVisualStyleBackColor = true;
            // 
            // ucModels
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(852, 506);
            Controls.Add(splitContainer1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ucModels";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "模型管理";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private TreeView tvModels;
        private Panel panel1;
        private Button btnDeleteAgent;
        private Button button2;
        private Button btnAddAgent;
        private Panel panel2;
        private Button btnCancel;
        private Button btnAccept;
        private Button button1;
        private Button button3;
        private Button button4;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private CheckBox chkDefault;
        private TextBox txtDescription;
        private TextBox txtVersion;
        private TextBox txtApiKey;
        private TextBox txtAccessUrl;
        private TextBox txtName;
    }
}