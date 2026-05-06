using ACL.business;
using ACL.business.project;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ACL.uc
{
    public partial class ProjectDataList : UserControl
    {
        Dictionary<string, TreeNode> filePaths;

        public ProjectDataList()
        {
            filePaths = new Dictionary<string, TreeNode>();
            InitializeComponent();
            this.Load += ProjectDataList_Load;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabControl tabContent { get; set; }

        private void ProjectDataList_Load(object? sender, EventArgs e)
        {
            var project = ProjectConfig.Current;
            if (project == null || project.Name == null || project.Name == string.Empty)
            {
                MessageBox.Show("设置项目", "请选择项目所在路径", MessageBoxButtons.OK, MessageBoxIcon.Information);
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var path = dialog.SelectedPath;
                    var name = Path.GetFileName(path);
                    ProjectConfig.Current = new ProjectConfigInfo() { Name = name, Directory = path };
                }
            }

            RefreshProjectTree();
            Context.Instance.FileChanged += OnFileChanged;
        }

        private void OnFileChanged(string file)
        {
            this.Invoke(() =>
            {
                RefreshProjectTree();
                if (filePaths.ContainsKey(file))
                {
                    var node = filePaths[file];
                    node.ExpandAll();

                    tvProject.SelectedNode = node;
                    OnShowFileContent(tvProject, EventArgs.Empty);
                }
            });
        }

        private void RefreshProjectTree()
        {
            filePaths.Clear();
            tvProject.Nodes.Clear();
            projectPath.Text = ProjectConfig.Current.Directory;
            ShowFilesToTree(ProjectConfig.Current.Directory, null);
        }


        private void ShowFilesToTree(string directory, TreeNode? parent)
        {
            if (!Directory.Exists(directory)) { return; }

            Directory.GetDirectories(directory).ToList().ForEach(dir =>
            {
                var node = new TreeNode() { Text = Path.GetFileName(dir) };
                node.Tag = new ProjectNodeTag
                {
                    Path = dir,
                    Name = node.Text,
                    IsDirectory = true
                };

                if (parent == null)
                    tvProject.Nodes.Add(node);
                else
                    parent.Nodes.Add(node);

                ShowFilesToTree(dir, node);
                node.ImageIndex = 0;
            });

            Directory.GetFiles(directory).ToList().ForEach(file =>
            {
                var fileNode = new TreeNode() { Text = Path.GetFileName(file) };

                if (parent == null)
                    tvProject.Nodes.Add(fileNode);
                else
                    parent.Nodes.Add(fileNode);
                fileNode.Tag = new ProjectNodeTag
                {
                    Path = file,
                    Name = fileNode.Text,
                    IsDirectory = false
                };

                fileNode.ImageIndex = 1;


                this.filePaths.Add(file, fileNode);
            });
        }


        private void tvProject_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;
            var tag = e.Node.Tag as ProjectNodeTag;
            if (tag == null) return;
            e.Node.ImageIndex = tag.IsDirectory ? 0 : 1;
            tvProject.SelectedImageIndex = e.Node.ImageIndex;
        }

        private void OnShowFileContent(object sender, EventArgs e)
        {
            var node = tvProject.SelectedNode;
            if (node == null) return;

            var tag = node.Tag as ProjectNodeTag;
            if (tag == null) return;


            if (tag.IsDirectory) return;

            var file = tag.Path;

            var closedType = typeof(ClosedTab);
            foreach (var item in tabContent.TabPages)
            {
                if (item == null) continue;
                if (item.GetType() != closedType) continue;
                var closedTab = item as ClosedTab;
                if (closedTab == null) continue;

                if (closedTab.FileName == file)
                {
                    tabContent.SelectedTab = closedTab;
                    return;
                }
            }

            var closed = new ClosedTab();
            closed.FileName = file;
            closed.Text = Path.GetFileName(file);
            tabContent.TabPages.Add(closed);
            tabContent.SelectedTab = closed;
        }


        private class ProjectNodeTag
        {
            public string Path { get; set; }
            public string Name { get; set; }
            public bool IsDirectory { get; set; } = false;
        }

        private void OnSelectPath(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var path = dialog.SelectedPath;
                var name = Path.GetFileName(path);
                ProjectConfig.Current = new ProjectConfigInfo() { Name = name, Directory = path };
                RefreshProjectTree();
            }
        }

        private void OnRefreshProject(object sender, EventArgs e)
        {
            RefreshProjectTree();
        }

        private void OnOpenFolder(object sender, EventArgs e)
        {
            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo()
            {
                FileName = "explorer.exe",
                Arguments = ProjectConfig.Current.Directory,
                CreateNoWindow = true,
            };
            proc.Start();
        }

        private void OnOpenFile(object sender, EventArgs e)
        {
            var tv = this.tvProject.SelectedNode;
            if (tv == null) return;
            var tag = tv.Tag as ProjectNodeTag;
            if (tag == null) return;

            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo()
            {
                FileName = "explorer.exe",
                Arguments = tag.Path,
                CreateNoWindow = true,
            };
            proc.Start();
        }

        private void OnNewFolder(object sender, EventArgs e)
        {
            var frmFile = new NewFile();
            frmFile.FileType = NewFile.Type.Directory;
            if (frmFile.ShowDialog() == DialogResult.OK)
            {
                RefreshProjectTree();
            }
        }

        private void OnNewFile(object sender, EventArgs e)
        {
            var frmFile = new NewFile();
            frmFile.FileType = NewFile.Type.File;
            if (frmFile.ShowDialog() == DialogResult.OK)
            {
                RefreshProjectTree();
            }
        }
    }
}
