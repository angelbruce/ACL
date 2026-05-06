namespace ACL.uc
{
    partial class EditFlow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditFlow));
            panel1 = new Panel();
            btnCancel = new Button();
            btnConfirm = new Button();
            panel2 = new Panel();
            txtDef = new RichTextBox();
            txtName = new TextBox();
            label2 = new Label();
            label1 = new Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(btnCancel);
            panel1.Controls.Add(btnConfirm);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 405);
            panel1.Name = "panel1";
            panel1.Size = new Size(599, 36);
            panel1.TabIndex = 0;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Image = (Image)resources.GetObject("btnCancel.Image");
            btnCancel.Location = new Point(451, 7);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(58, 23);
            btnCancel.TabIndex = 0;
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnConfirm
            // 
            btnConfirm.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            btnConfirm.Image = (Image)resources.GetObject("btnConfirm.Image");
            btnConfirm.Location = new Point(532, 7);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new Size(58, 23);
            btnConfirm.TabIndex = 0;
            btnConfirm.UseVisualStyleBackColor = true;
            btnConfirm.Click += btnConfirm_Click;
            // 
            // panel2
            // 
            panel2.Controls.Add(txtDef);
            panel2.Controls.Add(txtName);
            panel2.Controls.Add(label1);
            panel2.Controls.Add(label2);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(599, 405);
            panel2.TabIndex = 1;
            // 
            // txtDef
            // 
            txtDef.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtDef.Location = new Point(48, 41);
            txtDef.Name = "txtDef";
            txtDef.Size = new Size(524, 338);
            txtDef.TabIndex = 7;
            txtDef.Text = "";
            // 
            // txtName
            // 
            txtName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtName.Location = new Point(50, 12);
            txtName.Name = "txtName";
            txtName.Size = new Size(524, 23);
            txtName.TabIndex = 6;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 44);
            label2.Name = "label2";
            label2.Size = new Size(32, 17);
            label2.TabIndex = 5;
            label2.Text = "描述";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 16);
            label1.Name = "label1";
            label1.Size = new Size(32, 17);
            label1.TabIndex = 4;
            label1.Text = "名称";
            // 
            // NewFlow
            // 
            AcceptButton = btnConfirm;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(599, 441);
            ControlBox = false;
            Controls.Add(panel2);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            HelpButton = true;
            Name = "NewFlow";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "请输入会话内容";
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Button btnCancel;
        private Button btnConfirm;
        private RichTextBox txtDef;
        private TextBox txtName;
        private Label label1;
        private Label label2;
    }
}