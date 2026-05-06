using ABL;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;

namespace ACL.uc
{
    public partial class ClosedTab : TabPage
    {
        private Label label;
        private WebView2 view;
        static string[] AVAIL_CODE_STYLE = { "vs", "vs2015", "github", "github-dark", "atom-one-dark", "atom-one-light", "monokai" };

        public ClosedTab()
        {
            label = new Label();
            view = new WebView2();
            InitializeComponent();
            Load();
        }


        public ClosedTab(IContainer container)
        {
            label = new Label();
            view = new WebView2();
            container.Add(this);

            InitializeComponent();

            Load();
        }

        private void Load()
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("除此之外全部关闭", null, (item, e) =>
            {
                var tab = this.Parent as TabControl;
                foreach (TabPage page in tab.TabPages)
                {
                    if (page.GetType() != typeof(ClosedTab)) continue;
                    if (page == this) continue;
                    {
                        tab.TabPages.Remove(page);
                    }

                    tab.SelectedTab = this;
                }
            });
            menu.Items.Add("全部关闭", null, (item, e) =>
            {
                var tab = this.Parent as TabControl;
                foreach (TabPage page in tab.TabPages)
                {
                    if (page.GetType() != typeof(ClosedTab)) continue;
                    tab.TabPages.Remove(page);
                }

                tab.SelectedIndex = 0;
            });
            menu.Items.Add("关闭左侧全部", null, (item, e) =>
            {
                var tab = this.Parent as TabControl;
                var idx = tab.TabPages.IndexOf(this);
                for (int i = 0; i < idx; i++)
                {
                    var page = tab.TabPages[i];
                    if (page.GetType() != typeof(ClosedTab)) continue;
                    tab.TabPages.Remove(page);
                    i--;
                }

                tab.SelectedTab = this;
            });
            menu.Items.Add("关闭右侧全部", null, (item, e) =>
            {
                var tab = this.Parent as TabControl;
                var idx = tab.TabPages.IndexOf(this);
                for (int i = idx + 1; i < tab.TabPages.Count; i++)
                {
                    var page = tab.TabPages[i];
                    if (page.GetType() != typeof(ClosedTab)) continue;
                    tab.TabPages.Remove(page);
                    i--;
                }

                tab.SelectedTab = this;
            });
            menu.Items.Add("关闭", null, (item, e) =>
            {
                var tab = this.Parent as TabControl;
                tab.TabPages.Remove(this);
                tab.SelectedIndex = 0;
            });

            var panel = new Panel();
            panel.Dock = DockStyle.Top;
            panel.Height = 16;
            panel.ContextMenuStrip = menu;

            var pic = new PictureBox();
            pic.Height = 16;
            pic.Width = 16;
            pic.Dock = DockStyle.Right;
            pic.Image = imageList1.Images[0];
            panel.Controls.Add(pic);

            label.Dock = DockStyle.Fill;
            panel.Controls.Add(label);

            this.Controls.Add(panel);


            view.Dock = DockStyle.Fill;
            this.Controls.Add(view);


            pic.Click += OnCloseThis;

            this.Refresh();
        }

        private void OnCloseThis(object? sender, EventArgs e)
        {
            var parent = this.Parent as TabControl;
            if (parent != null)
            {
                parent.TabPages.Remove(this);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FileName
        {
            get
            {
                return label.Text;
            }
            set
            {
                label.Text = value;

                ShowFileContent(value);
            }
        }

        private async void ShowFileContent(string filename)
        {
            var extension = Path.GetExtension(filename);
            if (extension == null) extension = ".txt";
            extension = extension.ToLower();
            var fileType = GetFileType(extension);

            var lang = AVAIL_CODE_STYLE[1]; // 选择一个默认的代码高亮样式
            var cwd = System.AppDomain.CurrentDomain.BaseDirectory;
            var template = File.ReadAllText(Path.Combine(cwd, GetTemplate(fileType)));
            var buffer = File.Exists(filename) ? File.ReadAllBytes(filename) : System.Text.Encoding.UTF8.GetBytes("你所访问的文件不存在!");
            var envs = new Dictionary<string, string>
            {
                { "Path", cwd },
                { "Language", lang }
            };

            var content = GetShowText(fileType, buffer, extension);
            envs.Add("Code", content);
            foreach (var item in envs)
            {
                template = template.Replace($"${{{item.Key}}}", item.Value);
            }

            var options = new CoreWebView2EnvironmentOptions
            {
                AdditionalBrowserArguments = "--allow-file-access-from-files"
            };

            var env = await CoreWebView2Environment.CreateAsync(browserExecutableFolder: null, userDataFolder: cwd, options: options);
            await view.EnsureCoreWebView2Async(env);
            view.CoreWebView2.SetVirtualHostNameToFolderMapping("local.res", cwd, CoreWebView2HostResourceAccessKind.Allow);
            view.NavigateToString(template);
        }


        private string GetShowText(FileType fileType, byte[] content, string extension)
        {
            switch (fileType)
            {
                case FileType.Text:
                    return HtmlEncoder.Default.Encode(System.Text.Encoding.UTF8.GetString(content));

                case FileType.Image:
                    var mime = "image/png";
                    switch (extension)
                    {
                        case ".png": mime = "image/png"; break;
                        case ".jpg": mime = "image/jpg"; break;
                        case ".bmp": mime = "image/bmp"; break;
                        case ".ico": mime = "image/ico"; break;
                        case ".jpeg": mime = "image/jpeg"; break;
                        case ".svg": mime = "image/svg+xml"; break;
                        case ".gif": mime = "image/gif"; break;
                        default: mime = "image/png"; break;
                    }
                    return $"data:{mime};base64,{Convert.ToBase64String(content)}";

                case FileType.Other:
                default:
                    return System.Text.Encoding.UTF8.GetString(content);
            }
        }


        private string GetTemplate(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Text:
                    return "web/code.html";

                case FileType.Image:
                    return "web/image.html";

                case FileType.Other:
                default:
                    return "web/code.html";
            }
        }


        private FileType GetFileType(string extension)
        {
            switch (extension)
            {
                case ".xml":
                case ".json":
                case ".yml":
                case ".yaml":
                case ".boml":
                case ".bson":
                case ".doc":
                case ".docx":
                case ".xls":
                case ".xlsx":
                case ".ppt":
                case ".md":
                case ".txt":
                case ".cs":
                case ".java":
                case ".py":
                case ".kt":
                case ".sc":
                case ".sql":
                case ".dart":
                case ".vb":
                case ".vbs":
                case ".as":
                case ".rt":
                case ".js":
                case ".css":
                case ".ts":
                case ".less":
                case ".scss":
                case ".sh":
                case ".bat":
                case ".vs":
                case ".shell":
                case ".proj":
                case ".csproj":
                case ".vsproj":
                case ".slnx":
                case ".user":
                case ".lnk":
                case ".tpl":
                case ".st":
                case ".html":
                case ".htm":
                case ".shtml":
                case ".asp":
                case ".aspx":
                case ".jsp":
                case ".jspx":
                case ".csp":
                case ".log":
                case ".dot":
                case ".build":
                case ".k8s":
                    return FileType.Text;

                case ".png":
                case ".jpg":
                case ".bmp":
                case ".ico":
                case ".jpeg":
                case ".gif":
                case ".svg":
                    return FileType.Image;


                default:
                    return FileType.Other;
            }
        }

        public enum FileType
        {
            Text,
            Image,
            Other
        }


    }
}
