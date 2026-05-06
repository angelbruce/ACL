using ACL.web;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace ACL.uc
{
    public partial class ClosedPageTab : UserControl
    {
        const string prefixWeb = "http://local.res/web/";
        const string prefix = "http://local.res/";

        private string JSON_TYPE = "application/json;charset=utf-8";

        private static readonly Dictionary<string, string> _mimeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // 文本类
            { ".html", "text/html" },
            { ".htm", "text/html" },
            { ".css", "text/css" },
            { ".js", "application/javascript" },
            { ".json", "application/json" },
            { ".xml", "application/xml" },
            { ".txt", "text/plain" },
            { ".csv", "text/csv" },
        
            // 图片类
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" },
            { ".gif", "image/gif" },
            { ".svg", "image/svg+xml" },
            { ".ico", "image/x-icon" },
            { ".webp", "image/webp" },
        
            // 字体类
            { ".woff", "font/woff" },
            { ".woff2", "font/woff2" },
            { ".ttf", "font/ttf" },
            { ".eot", "application/vnd.ms-fontobject" },
        
            // 音频视频
            { ".mp3", "audio/mpeg" },
            { ".wav", "audio/wav" },
            { ".mp4", "video/mp4" },
            { ".webm", "video/webm" },
            { ".ogg", "video/ogg" },
        
            // 文档与压缩
            { ".pdf", "application/pdf" },
            { ".zip", "application/zip" },
            { ".rar", "application/x-rar-compressed" },
            { ".doc", "application/msword" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".xls", "application/vnd.ms-excel" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" }
        };

        private string flowId;
        private WebView2 view;

        public ClosedPageTab()
        {
            view = new WebView2();
            InitializeComponent();
            Load();
        }


        public ClosedPageTab(IContainer container)
        {
            view = new WebView2();
            container.Add(this);

            InitializeComponent();

            Load();
        }

        private void Load()
        {
            view.Dock = DockStyle.Fill;
            this.Controls.Add(view);
            this.Refresh();
        }

        private void OnCloseThis(object? sender, EventArgs e)
        {
            var parent = this.Parent as TabControl;
            if (parent != null)
            {
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FlowId
        {
            get
            {
                return flowId;
            }
            set
            {
                this.flowId = value;
                ShowFileContent("web/flow.html");
            }
        }


        private async void ShowFileContent(string filename)
        {
            var cwd = AppDomain.CurrentDomain.BaseDirectory;
            var options = new CoreWebView2EnvironmentOptions
            {
                AdditionalBrowserArguments = "--allow-file-access-from-files"
            };

            var env = await CoreWebView2Environment.CreateAsync(browserExecutableFolder: null, userDataFolder: cwd, options: options);
            await view.EnsureCoreWebView2Async(env);

            view.CoreWebView2.AddWebResourceRequestedFilter("http://local.res/*", CoreWebView2WebResourceContext.All);
            view.CoreWebView2.WebResourceRequested += (o, e) =>
            {

                if (e.Request.Uri.StartsWith(prefixWeb))
                {
                    var path = AppDomain.CurrentDomain.BaseDirectory;
                    var url = e.Request.Uri.ToString().Substring(prefix.Length);
                    var idx = url.IndexOf('?');
                    if (idx != -1)
                    {
                        url = url.Substring(0, idx);
                    }

                    var file = Path.Combine(path, url);

                    var type = GetContentType(file);
                    var buffer = File.ReadAllBytes(file);
                    var stream = new MemoryStream(buffer);
                    var response = view.CoreWebView2.Environment.CreateWebResourceResponse(stream, 200, "OK", $"Content-Type: {type}");
                    e.Response = response;
                    e.Response.Headers.AppendHeader("Access-Control-Allow-Origin", "*");   // 允许所有源
                    e.Response.Headers.AppendHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS, PUT, DELETE");
                    e.Response.Headers.AppendHeader("Access-Control-Allow-Headers", "Content-Type");
                }
                else
                {
                    Invoke(e);
                }
                Trace.WriteLine(e.Request.Uri + "\n" + e.Request.Content);
            };

            view.CoreWebView2.Navigate("http://local.res/web/flow.html?flowId=" + flowId);
        }



        private void Invoke(CoreWebView2WebResourceRequestedEventArgs e)
        {
            var url = e.Request.Uri;
            var action = url;
            if (url.IndexOf(prefix) != -1)
            {
                action = url.Substring(prefix.Length);
                var methods = typeof(DataController).GetMethods();
                MethodInfo? method = null;
                foreach (var item in methods)
                {
                    if (item == null || item.IsPrivate) continue;
                    var actionAttr = item.GetCustomAttribute<ActionAttribute>();
                    if (actionAttr == null) continue;

                    if (actionAttr.Name.Equals(action, StringComparison.OrdinalIgnoreCase))
                    {
                        method = item;
                        break;
                    }
                }

                if (method == null)
                {
                    var error = $"{{\"message\":\"no method to response\"}}";
                    e.Response = JSON(error);
                    return;
                }

                var content = string.Empty;
                if (e.Request.Content != null)
                {
                    using (var reader = new StreamReader(e.Request.Content))
                    {
                        content = reader.ReadToEnd();
                    }
                }

                if (content == null) content = "{}";
                var paremeters = method.GetParameters();
                var datas = new object[paremeters.Length];
                if (paremeters.Length > 0)
                {
                    var data = JsonConvert.DeserializeObject(content, paremeters[0].ParameterType);
                    if (data == null)
                    {
                        var error = $"{{\"message\":\"request deserialize failed when call method,please check.\"}}";
                        e.Response = JSON(error);
                        return;
                    }

                    datas[0] = data;
                }

                var controller = new DataController();
                var obj = method.Invoke(controller, datas);
                var json = JsonConvert.SerializeObject(obj);
                e.Response = JSON(json);
                e.Response.Headers.AppendHeader("Access-Control-Allow-Origin", "*");   // 允许所有源
                e.Response.Headers.AppendHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS, PUT, DELETE");
                e.Response.Headers.AppendHeader("Access-Control-Allow-Headers", "Content-Type");
            }
        }



        private CoreWebView2WebResourceResponse JSON(string json)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return view.CoreWebView2.Environment.CreateWebResourceResponse(stream, 200, "OK", $"Content-Type: {JSON_TYPE}");
        }

        /// <summary>
        /// 根据文件扩展名获取 MIME Type
        /// </summary>
        /// <param name="fileName">文件名或路径</param>
        /// <returns>MIME Type，如果未知则返回 application/octet-stream</returns>
        private static string GetContentType(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "application/octet-stream";

            var extension = System.IO.Path.GetExtension(fileName);

            if (string.IsNullOrEmpty(extension))
                return "application/octet-stream";

            extension = extension.ToLower();
            if (_mimeMap.TryGetValue(extension, out var contentType))
            {
                return contentType;
            }

            // 未知类型默认返回二进制流
            return "application/octet-stream";
        }
    }
}
