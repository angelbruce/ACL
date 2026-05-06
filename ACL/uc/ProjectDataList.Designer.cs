namespace ACL.uc
{
    partial class ProjectDataList
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectDataList));
            panel4 = new Panel();
            button1 = new Button();
            projectPath = new LinkLabel();
            imageList1 = new ImageList(components);
            tvProject = new TreeView();
            contextMenuStrip1 = new ContextMenuStrip(components);
            打开项目目录ToolStripMenuItem = new ToolStripMenuItem();
            打开ToolStripMenuItem = new ToolStripMenuItem();
            新建文件ToolStripMenuItem = new ToolStripMenuItem();
            新建目录ToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            panel4.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panel4
            // 
            panel4.Controls.Add(button1);
            panel4.Controls.Add(projectPath);
            panel4.Dock = DockStyle.Top;
            panel4.Location = new Point(0, 0);
            panel4.Name = "panel4";
            panel4.Size = new Size(255, 23);
            panel4.TabIndex = 1;
            // 
            // button1
            // 
            button1.Dock = DockStyle.Right;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Image = (Image)resources.GetObject("button1.Image");
            button1.Location = new Point(226, 0);
            button1.Name = "button1";
            button1.Size = new Size(29, 23);
            button1.TabIndex = 1;
            button1.UseVisualStyleBackColor = true;
            button1.Click += OnRefreshProject;
            // 
            // projectPath
            // 
            projectPath.AutoSize = true;
            projectPath.Location = new Point(5, 2);
            projectPath.Name = "projectPath";
            projectPath.Size = new Size(0, 17);
            projectPath.TabIndex = 0;
            projectPath.VisitedLinkColor = Color.Blue;
            projectPath.Click += OnSelectPath;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "png-0033.png");
            imageList1.Images.SetKeyName(1, "png-1101.png");
            // 
            // tvProject
            // 
            tvProject.ContextMenuStrip = contextMenuStrip1;
            tvProject.Dock = DockStyle.Fill;
            tvProject.FullRowSelect = true;
            tvProject.ImageIndex = 0;
            tvProject.ImageList = imageList1;
            tvProject.Location = new Point(0, 23);
            tvProject.Name = "tvProject";
            tvProject.SelectedImageIndex = 0;
            tvProject.Size = new Size(255, 502);
            tvProject.TabIndex = 3;
            tvProject.AfterSelect += tvProject_AfterSelect;
            tvProject.DoubleClick += OnShowFileContent;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { 打开项目目录ToolStripMenuItem, 打开ToolStripMenuItem, toolStripMenuItem1, 新建目录ToolStripMenuItem, 新建文件ToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(181, 120);
            // 
            // 打开项目目录ToolStripMenuItem
            // 
            打开项目目录ToolStripMenuItem.Name = "打开项目目录ToolStripMenuItem";
            打开项目目录ToolStripMenuItem.Size = new Size(180, 22);
            打开项目目录ToolStripMenuItem.Text = "打开项目目录";
            打开项目目录ToolStripMenuItem.Click += OnOpenFolder;
            // 
            // 打开ToolStripMenuItem
            // 
            打开ToolStripMenuItem.Name = "打开ToolStripMenuItem";
            打开ToolStripMenuItem.Size = new Size(180, 22);
            打开ToolStripMenuItem.Text = "打开";
            打开ToolStripMenuItem.Click += OnOpenFile;
            // 
            // 新建文件ToolStripMenuItem
            // 
            新建文件ToolStripMenuItem.Name = "新建文件ToolStripMenuItem";
            新建文件ToolStripMenuItem.Size = new Size(180, 22);
            新建文件ToolStripMenuItem.Text = "新建文件";
            新建文件ToolStripMenuItem.Click += OnNewFile;
            // 
            // 新建目录ToolStripMenuItem
            // 
            新建目录ToolStripMenuItem.Name = "新建目录ToolStripMenuItem";
            新建目录ToolStripMenuItem.Size = new Size(180, 22);
            新建目录ToolStripMenuItem.Text = "新建目录";
            新建目录ToolStripMenuItem.Click += OnNewFolder;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(177, 6);
            // 
            // ProjectDataList
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tvProject);
            Controls.Add(panel4);
            Name = "ProjectDataList";
            Size = new Size(255, 525);
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel4;
        private LinkLabel projectPath;
        private ImageList imageList1;
        private Button button1;
        private TreeView tvProject;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem 打开项目目录ToolStripMenuItem;
        private ToolStripMenuItem 打开ToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem 新建目录ToolStripMenuItem;
        private ToolStripMenuItem 新建文件ToolStripMenuItem;
    }
}
