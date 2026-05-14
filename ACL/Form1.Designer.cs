namespace ACL
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            toolStrip1 = new ToolStrip();
            tsbModel = new ToolStripButton();
            tsbMcpServers = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            statusStrip1 = new StatusStrip();
            tsdModels = new ToolStripDropDownButton();
            txtModel = new ToolStripStatusLabel();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            tsbAgent = new ToolStripDropDownButton();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            cmbUsage = new ToolStripDropDownButton();
            lblSession = new ToolStripStatusLabel();
            lblProject = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            tab = new TabControl();
            tabSession = new TabPage();
            tabProject = new TabPage();
            tabMcp = new TabPage();
            tvMcpTools = new TreeView();
            tabAgent = new TabPage();
            tabContent = new TabControl();
            tabPage1 = new TabPage();
            content = new RichTextBox();
            tabPage2 = new TabPage();
            txtDebug = new RichTextBox();
            tabPlan = new TabPage();
            taskDataList1 = new ACL.uc.TaskDataList();
            tabFlow = new TabPage();
            splitContainer2 = new SplitContainer();
            imageList1 = new ImageList(components);
            tabControl1 = new TabControl();
            tabPage3 = new TabPage();
            tabPage4 = new TabPage();
            panelFlowInfo = new Panel();
            pnlOutput = new Panel();
            toolStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tab.SuspendLayout();
            tabMcp.SuspendLayout();
            tabContent.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPlan.SuspendLayout();
            tabFlow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage4.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { tsbModel, tsbMcpServers, toolStripSeparator1 });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(918, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // tsbModel
            // 
            tsbModel.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbModel.Image = (Image)resources.GetObject("tsbModel.Image");
            tsbModel.ImageTransparentColor = Color.Magenta;
            tsbModel.Name = "tsbModel";
            tsbModel.Size = new Size(36, 22);
            tsbModel.Text = "模型";
            tsbModel.Click += tsbModel_Click;
            // 
            // tsbMcpServers
            // 
            tsbMcpServers.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbMcpServers.Image = (Image)resources.GetObject("tsbMcpServers.Image");
            tsbMcpServers.ImageTransparentColor = Color.Magenta;
            tsbMcpServers.Name = "tsbMcpServers";
            tsbMcpServers.Size = new Size(86, 22);
            tsbMcpServers.Text = "MCP Servers";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { tsdModels, txtModel, toolStripStatusLabel1, tsbAgent, toolStripStatusLabel2, cmbUsage, lblSession, lblProject });
            statusStrip1.Location = new Point(0, 631);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(918, 23);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // tsdModels
            // 
            tsdModels.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsdModels.Image = (Image)resources.GetObject("tsdModels.Image");
            tsdModels.ImageTransparentColor = Color.Magenta;
            tsdModels.Name = "tsdModels";
            tsdModels.Size = new Size(13, 21);
            // 
            // txtModel
            // 
            txtModel.Name = "txtModel";
            txtModel.Size = new Size(45, 18);
            txtModel.Text = "model";
            txtModel.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.ForeColor = SystemColors.ControlDark;
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(11, 18);
            toolStripStatusLabel1.Text = "|";
            // 
            // tsbAgent
            // 
            tsbAgent.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbAgent.Image = (Image)resources.GetObject("tsbAgent.Image");
            tsbAgent.ImageTransparentColor = Color.Magenta;
            tsbAgent.Name = "tsbAgent";
            tsbAgent.Size = new Size(54, 21);
            tsbAgent.Text = "agent";
            tsbAgent.DropDownItemClicked += tsbAgent_DropDownItemClicked;
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.ForeColor = SystemColors.ControlDark;
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(11, 18);
            toolStripStatusLabel2.Text = "|";
            // 
            // cmbUsage
            // 
            cmbUsage.DisplayStyle = ToolStripItemDisplayStyle.Text;
            cmbUsage.Image = (Image)resources.GetObject("cmbUsage.Image");
            cmbUsage.ImageTransparentColor = Color.Magenta;
            cmbUsage.Name = "cmbUsage";
            cmbUsage.Size = new Size(56, 21);
            cmbUsage.Text = "usage";
            cmbUsage.Visible = false;
            cmbUsage.DropDownItemClicked += OnChoseUsage;
            // 
            // lblSession
            // 
            lblSession.Name = "lblSession";
            lblSession.Size = new Size(0, 18);
            // 
            // lblProject
            // 
            lblProject.Name = "lblProject";
            lblProject.Size = new Size(0, 18);
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.Location = new Point(0, 25);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tab);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tabContent);
            splitContainer1.Size = new Size(918, 606);
            splitContainer1.SplitterDistance = 267;
            splitContainer1.TabIndex = 3;
            // 
            // tab
            // 
            tab.Controls.Add(tabSession);
            tab.Controls.Add(tabProject);
            tab.Controls.Add(tabMcp);
            tab.Controls.Add(tabAgent);
            tab.Dock = DockStyle.Fill;
            tab.Location = new Point(0, 0);
            tab.Name = "tab";
            tab.SelectedIndex = 0;
            tab.Size = new Size(267, 606);
            tab.TabIndex = 2;
            // 
            // tabSession
            // 
            tabSession.Location = new Point(4, 26);
            tabSession.Name = "tabSession";
            tabSession.Padding = new Padding(3);
            tabSession.Size = new Size(259, 576);
            tabSession.TabIndex = 0;
            tabSession.Text = "会话";
            tabSession.UseVisualStyleBackColor = true;
            // 
            // tabProject
            // 
            tabProject.Location = new Point(4, 26);
            tabProject.Name = "tabProject";
            tabProject.Padding = new Padding(3);
            tabProject.Size = new Size(259, 576);
            tabProject.TabIndex = 2;
            tabProject.Text = "项目";
            tabProject.UseVisualStyleBackColor = true;
            // 
            // tabMcp
            // 
            tabMcp.Controls.Add(tvMcpTools);
            tabMcp.Location = new Point(4, 26);
            tabMcp.Name = "tabMcp";
            tabMcp.Padding = new Padding(3);
            tabMcp.Size = new Size(259, 576);
            tabMcp.TabIndex = 1;
            tabMcp.Text = "MCP";
            tabMcp.UseVisualStyleBackColor = true;
            // 
            // tvMcpTools
            // 
            tvMcpTools.Dock = DockStyle.Fill;
            tvMcpTools.Location = new Point(3, 3);
            tvMcpTools.Name = "tvMcpTools";
            tvMcpTools.Size = new Size(253, 570);
            tvMcpTools.TabIndex = 0;
            // 
            // tabAgent
            // 
            tabAgent.Location = new Point(4, 26);
            tabAgent.Name = "tabAgent";
            tabAgent.Padding = new Padding(3);
            tabAgent.Size = new Size(259, 576);
            tabAgent.TabIndex = 3;
            tabAgent.Text = "Agent";
            tabAgent.UseVisualStyleBackColor = true;
            // 
            // tabContent
            // 
            tabContent.Controls.Add(tabPage1);
            tabContent.Controls.Add(tabPage2);
            tabContent.Controls.Add(tabPlan);
            tabContent.Controls.Add(tabFlow);
            tabContent.Dock = DockStyle.Fill;
            tabContent.Location = new Point(0, 0);
            tabContent.Name = "tabContent";
            tabContent.SelectedIndex = 0;
            tabContent.Size = new Size(647, 606);
            tabContent.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(content);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(639, 576);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "编辑器";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // content
            // 
            content.AcceptsTab = true;
            content.AutoWordSelection = true;
            content.Dock = DockStyle.Fill;
            content.Location = new Point(3, 3);
            content.Name = "content";
            content.ReadOnly = true;
            content.Size = new Size(633, 570);
            content.TabIndex = 1;
            content.Text = "";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(txtDebug);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(639, 576);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "DEBUG";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // txtDebug
            // 
            txtDebug.AcceptsTab = true;
            txtDebug.AutoWordSelection = true;
            txtDebug.Dock = DockStyle.Fill;
            txtDebug.Location = new Point(3, 3);
            txtDebug.Name = "txtDebug";
            txtDebug.ReadOnly = true;
            txtDebug.Size = new Size(633, 570);
            txtDebug.TabIndex = 2;
            txtDebug.Text = "";
            // 
            // tabPlan
            // 
            tabPlan.Controls.Add(taskDataList1);
            tabPlan.Location = new Point(4, 26);
            tabPlan.Name = "tabPlan";
            tabPlan.Padding = new Padding(3);
            tabPlan.Size = new Size(639, 576);
            tabPlan.TabIndex = 2;
            tabPlan.Text = "执行计划";
            tabPlan.UseVisualStyleBackColor = true;
            // 
            // taskDataList1
            // 
            taskDataList1.Dock = DockStyle.Fill;
            taskDataList1.Location = new Point(3, 3);
            taskDataList1.Name = "taskDataList1";
            taskDataList1.Size = new Size(633, 570);
            taskDataList1.TabIndex = 0;
            // 
            // tabFlow
            // 
            tabFlow.Controls.Add(splitContainer2);
            tabFlow.Location = new Point(4, 26);
            tabFlow.Name = "tabFlow";
            tabFlow.Padding = new Padding(3);
            tabFlow.Size = new Size(639, 576);
            tabFlow.TabIndex = 3;
            tabFlow.Text = "工作流";
            tabFlow.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.FixedPanel = FixedPanel.Panel1;
            splitContainer2.Location = new Point(3, 3);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(tabControl1);
            splitContainer2.Size = new Size(633, 570);
            splitContainer2.SplitterDistance = 255;
            splitContainer2.TabIndex = 0;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "png-0033.png");
            imageList1.Images.SetKeyName(1, "png-1101.png");
            // 
            // tabControl1
            // 
            tabControl1.Alignment = TabAlignment.Bottom;
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(374, 570);
            tabControl1.TabIndex = 0;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(panelFlowInfo);
            tabPage3.Location = new Point(4, 4);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(366, 540);
            tabPage3.TabIndex = 0;
            tabPage3.Text = "流程信息";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(pnlOutput);
            tabPage4.Location = new Point(4, 4);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3);
            tabPage4.Size = new Size(366, 540);
            tabPage4.TabIndex = 1;
            tabPage4.Text = "输出";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // panelFlowInfo
            // 
            panelFlowInfo.Dock = DockStyle.Fill;
            panelFlowInfo.Location = new Point(3, 3);
            panelFlowInfo.Name = "panelFlowInfo";
            panelFlowInfo.Size = new Size(360, 534);
            panelFlowInfo.TabIndex = 0;
            // 
            // pnlOutput
            // 
            pnlOutput.Dock = DockStyle.Fill;
            pnlOutput.Location = new Point(3, 3);
            pnlOutput.Name = "pnlOutput";
            pnlOutput.Size = new Size(360, 534);
            pnlOutput.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(918, 654);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AGENT控制流";
            WindowState = FormWindowState.Maximized;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tab.ResumeLayout(false);
            tabMcp.ResumeLayout(false);
            tabContent.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPlan.ResumeLayout(false);
            tabFlow.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            tabPage4.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem msbAbort;
        private ToolStrip toolStrip1;
        private StatusStrip statusStrip1;
        private SplitContainer splitContainer1;
        private TabControl tab;
        private TabPage tabMcp;
        private ToolStripButton tsbMcpServers;
        private ToolStripButton tsbModel;
        private TabPage tabProject;
        private TreeView treeView1;
        private TreeView tvMcpTools;
        private TabControl tabContent;
        private TabPage tabPage1;
        private RichTextBox content;
        private TabPage tabPage2;
        private RichTextBox txtDebug;
        private ImageList imageList1;
        private ToolStripStatusLabel txtModel;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripDropDownButton cmbUsage;
        private TabPage tabPlan;
        private uc.TaskDataList taskDataList1;
        private TabPage tabSession;
        private ToolStripSeparator toolStripSeparator1;
        private TabPage tabAgent;
        private TabPage tabFlow;
        private SplitContainer splitContainer2;
        private ToolStripDropDownButton tsbAgent;
        private ToolStripStatusLabel lblSession;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripStatusLabel lblProject;
        private ToolStripDropDownButton tsdModels;
        private TabControl tabControl1;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private Panel panelFlowInfo;
        private Panel pnlOutput;
    }
}
