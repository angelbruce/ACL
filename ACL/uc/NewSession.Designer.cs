namespace ACL.uc
{
    partial class NewSession
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewSession));
            panel1 = new Panel();
            panel2 = new Panel();
            btnConfirm = new Button();
            btnCancel = new Button();
            richTextBox1 = new RichTextBox();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(btnCancel);
            panel1.Controls.Add(btnConfirm);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 258);
            panel1.Name = "panel1";
            panel1.Size = new Size(511, 36);
            panel1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Controls.Add(richTextBox1);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(511, 258);
            panel2.TabIndex = 1;
            // 
            // btnConfirm
            // 
            btnConfirm.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            btnConfirm.Image = (Image)resources.GetObject("btnConfirm.Image");
            btnConfirm.Location = new Point(444, 7);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new Size(58, 23);
            btnConfirm.TabIndex = 0;
            btnConfirm.UseVisualStyleBackColor = true;
            btnConfirm.Click += btnConfirm_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Image = (Image)resources.GetObject("btnCancel.Image");
            btnCancel.Location = new Point(363, 7);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(58, 23);
            btnCancel.TabIndex = 0;
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            richTextBox1.Location = new Point(12, 12);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(487, 228);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "";
            // 
            // NewSession
            // 
            AcceptButton = btnConfirm;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(511, 294);
            ControlBox = false;
            Controls.Add(panel2);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            HelpButton = true;
            Name = "NewSession";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "请输入会话内容";
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Button btnCancel;
        private Button btnConfirm;
        private RichTextBox richTextBox1;
    }
}