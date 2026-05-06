namespace ACL.uc
{
    partial class SessionDataList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SessionDataList));
            toolStrip2 = new ToolStrip();
            tsbNewSession = new ToolStripButton();
            tsbDelete = new ToolStripButton();
            tsbCompress = new ToolStripButton();
            panel2 = new Panel();
            tvSessions = new TreeView();
            pnlAsk = new Panel();
            txtAsk = new RichTextBox();
            panel3 = new Panel();
            btnCancel = new Button();
            btnSend = new Button();
            toolStrip2.SuspendLayout();
            panel2.SuspendLayout();
            pnlAsk.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip2
            // 
            toolStrip2.Items.AddRange(new ToolStripItem[] { tsbNewSession, tsbDelete, tsbCompress });
            toolStrip2.Location = new Point(0, 0);
            toolStrip2.Name = "toolStrip2";
            toolStrip2.Size = new Size(280, 25);
            toolStrip2.TabIndex = 0;
            toolStrip2.Text = "toolStrip2";
            // 
            // tsbNewSession
            // 
            tsbNewSession.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbNewSession.Image = (Image)resources.GetObject("tsbNewSession.Image");
            tsbNewSession.ImageTransparentColor = Color.Magenta;
            tsbNewSession.Name = "tsbNewSession";
            tsbNewSession.Size = new Size(23, 22);
            tsbNewSession.Text = "创建一个新的会话";
            tsbNewSession.Click += OnCreateNewSession;
            // 
            // tsbDelete
            // 
            tsbDelete.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbDelete.Image = (Image)resources.GetObject("tsbDelete.Image");
            tsbDelete.ImageTransparentColor = Color.Magenta;
            tsbDelete.Name = "tsbDelete";
            tsbDelete.Size = new Size(23, 22);
            tsbDelete.Text = "删除此会话";
            tsbDelete.Click += OnDeleteNewSession;
            // 
            // tsbCompress
            // 
            tsbCompress.DisplayStyle = ToolStripItemDisplayStyle.Image;
            tsbCompress.Image = (Image)resources.GetObject("tsbCompress.Image");
            tsbCompress.ImageTransparentColor = Color.Magenta;
            tsbCompress.Name = "tsbCompress";
            tsbCompress.Size = new Size(23, 22);
            tsbCompress.Text = "会话上下文压缩";
            // 
            // panel2
            // 
            panel2.Controls.Add(tvSessions);
            panel2.Controls.Add(pnlAsk);
            panel2.Controls.Add(panel3);
            panel2.Controls.Add(toolStrip2);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(280, 644);
            panel2.TabIndex = 2;
            // 
            // tvSessions
            // 
            tvSessions.Dock = DockStyle.Fill;
            tvSessions.FullRowSelect = true;
            tvSessions.Location = new Point(0, 25);
            tvSessions.Name = "tvSessions";
            tvSessions.Size = new Size(280, 458);
            tvSessions.TabIndex = 13;
            tvSessions.AfterSelect += tvSessions_AfterSelect;
            // 
            // pnlAsk
            // 
            pnlAsk.Controls.Add(txtAsk);
            pnlAsk.Dock = DockStyle.Bottom;
            pnlAsk.Location = new Point(0, 483);
            pnlAsk.Name = "pnlAsk";
            pnlAsk.Size = new Size(280, 126);
            pnlAsk.TabIndex = 12;
            pnlAsk.Visible = false;
            // 
            // txtAsk
            // 
            txtAsk.Dock = DockStyle.Fill;
            txtAsk.Location = new Point(0, 0);
            txtAsk.Name = "txtAsk";
            txtAsk.Size = new Size(280, 126);
            txtAsk.TabIndex = 2;
            txtAsk.Text = "我的桌面C:\\Users\\angel\\Desktop有哪些txt文件呢？";
            txtAsk.KeyPress += txtAsk_KeyPress;
            // 
            // panel3
            // 
            panel3.Controls.Add(btnCancel);
            panel3.Controls.Add(btnSend);
            panel3.Dock = DockStyle.Bottom;
            panel3.Location = new Point(0, 609);
            panel3.Name = "panel3";
            panel3.Size = new Size(280, 35);
            panel3.TabIndex = 10;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            btnCancel.Location = new Point(15, 6);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "取消(&c)";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += OnCancelAsk;
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            btnSend.Location = new Point(192, 6);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(75, 23);
            btnSend.TabIndex = 0;
            btnSend.Text = "发送(&s)";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // SessionDataList
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panel2);
            Name = "SessionDataList";
            Size = new Size(280, 644);
            toolStrip2.ResumeLayout(false);
            toolStrip2.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            pnlAsk.ResumeLayout(false);
            panel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ToolStrip toolStrip2;
        private ToolStripButton tsbNewSession;
        private ToolStripButton tsbDelete;
        private ToolStripButton tsbCompress;
        private Panel panel2;
        private Panel panel3;
        private Button btnCancel;
        private Button btnSend;
        private Panel pnlAsk;
        private RichTextBox txtAsk;
        private TreeView tvSessions;
    }
}
