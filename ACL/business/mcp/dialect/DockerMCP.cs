using ACL.business.log;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ACL.business.mcp.dialect
{
    /// <summary>
    /// MCP 客户端 - 通过 stdio 连接到 MCP Gateway
    /// </summary>
    //[MCPFactory]
    public class DockerMCP : IMCPable, IDisposable
    {
        const string DOCKER_PREFIX = "__DOCKER__";
        private Process _process;
        private StreamWriter _stdin;
        private StreamReader _stdout;
        private int _requestId = 0;

        public bool Test(string fnName)
        {
            if (fnName == null || fnName.Length == 0)
            {
                return false;
            }

            return fnName.StartsWith(DOCKER_PREFIX);
        }

        /// <summary>
        /// 初始化 MCP 客户端并启动 Gateway
        /// </summary>
        public async Task<bool> InitializeAsync()
        {
            GlobalLogger.Debug("[*] 启动 MCP Gateway...");

            // 启动 docker mcp gateway run 进程
            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = "mcp gateway run",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                }
            };

            _process.Start();
            _stdin = _process.StandardInput;
            _stdout = _process.StandardOutput;

            GlobalLogger.Debug("[✓] MCP Gateway 已启动");

            // 等待 Gateway 初始化
            await Task.Delay(10000);
            return await Task.FromResult(true);
        }

        /// <summary>
        /// 初始化 MCP 会话
        /// </summary>
        public async Task<bool> SessionInitializeAsync()
        {
            GlobalLogger.Debug("[*] 初始化 MCP 会话...");

            var request = new MCPRequest
            {
                JsonRpc = "2.0",
                Id = GetNextRequestId(),
                Method = "initialize",
                Params = new
                {
                    protocolVersion = "2024-11-05",
                    capabilities = new { },
                    clientInfo = new
                    {
                        name = "csharp-client",
                        version = "1.0"
                    }
                }
            };

            var response = await SendRequestAsync(request);
            if (response != null && response.Result != null)
            {
                GlobalLogger.Debug("[✓] MCP 会话已初始化");
                return true;
            }

            GlobalLogger.Debug("[✗] 初始化失败");
            return true;
        }

        /// <summary>
        /// 列出所有可用工具
        /// </summary>
        public async Task<List<MCPTool>> ListToolsAsync()
        {
            GlobalLogger.Debug("[*] 获取工具列表...");

            var request = new MCPRequest
            {
                JsonRpc = "2.0",
                Id = GetNextRequestId(),
                Method = "tools/list",
                Params = new { }
            };

            var tools = new List<MCPTool>();
            var response = await SendRequestAsync(request);
            if (response != null && response.Result != null)
            {
                var result = response.Result.ToString();
                if (result != null && result.Length > 0)
                {
                    var toolList = JsonSerializer.Deserialize<ToolList>(result);
                    GlobalLogger.Debug($"[✓] 获取了 {toolList?.Tools?.Length ?? 0} 个工具");
                    if (toolList != null)
                    {
                        foreach (var item in toolList.Tools)
                        {
                            var tool = new MCPTool()
                            {
                                Name = $"{DOCKER_PREFIX}{item.Name}",
                                Description = item.Description,
                                InputSchema = BinaryData.FromString(item.InputSchema?.ToString()),
                            };

                            tools.Add(tool);
                        }
                    }
                }

                return await Task.FromResult(tools);
            }
            else
            {

                GlobalLogger.Debug("[✗] 获取工具列表失败");
                return await Task.FromResult(tools);
            }
        }

        /// <summary>
        /// 调用工具
        /// </summary>
        public async Task<MCPToolCallResult> CallToolAsync(string toolName, object arguments)
        {
            GlobalLogger.Debug($"[→] 调用工具: {toolName}");

            toolName = toolName.Substring(DOCKER_PREFIX.Length);

            GlobalLogger.Debug($"[→] 调用工具RealName is: {toolName}");
            var args = arguments != null ? JsonSerializer.Deserialize<Dictionary<string, object>>(arguments.ToString()) : new Dictionary<string, object>();
            if (args != null && args.Count > 0)
            {
                foreach (var arg in args)
                {
                    var val = arg.Value.ToString();
                    if (val != null && val.GetType() == typeof(string))
                    {
                        if (val.Contains(":"))
                        {
                            val = val.Replace("\\", "/").Replace(":", "").Insert(0, "/").Replace("\u0022", "").Replace("//", "/");
                            args[arg.Key] = val;
                        }
                    }
                }
            }

            var request = new MCPRequest
            {
                JsonRpc = "2.0",
                Id = GetNextRequestId(),
                Method = "tools/call",
                Params = new
                {
                    name = toolName,
                    arguments = args
                }
            };

            var response = await SendRequestAsync(request);
            if (response != null && response.Result != null)
            {
                var result = response.Result;
                GlobalLogger.Debug($"[←] 工具执行成功,${response}");
                return new MCPToolCallResult { Success = true, Content = result };
            }

            if (response?.Error != null)
            {
                GlobalLogger.Debug($"[✗] 工具执行失败: {response.Error}");
                return new MCPToolCallResult { Success = false, Error = response.Error.ToString() };
            }

            GlobalLogger.Debug($"[✗] 工具执行失败");
            return new MCPToolCallResult { Success = false };
        }

        /// <summary>
        /// 发送 MCP 请求并接收响应
        /// </summary>
        private async Task<MCPResponse> SendRequestAsync(MCPRequest request)
        {
            try
            {
                // 序列化请求为 JSON
                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                // 发送请求（行分隔 JSON）
                GlobalLogger.Debug($"DOCKER REQUEST : {json}");
                _stdin.WriteLine(json);
                await _stdin.FlushAsync();

                while (true)
                {
                    var timeout = 30;
                    try
                    {
                        using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout)))
                        {
                            GlobalLogger.Debug($"DOCKER REQUEST 开始获取响应。");
                            var responseLine = await _stdout.ReadLineAsync(cts.Token);
                            GlobalLogger.Debug($"DOCKER REQUEST 已经获取响应。");

                            if (string.IsNullOrEmpty(responseLine))
                            {
                                return null;
                            }

                            var response = JsonSerializer.Deserialize<MCPResponse>(responseLine);

                            // 如果是通知（没有 result/error），继续读下一条
                            if (response.Method != null && response.Id == null)
                            {
                                GlobalLogger.Debug($"[通知] {response.Method}");
                                continue;  // ← 忽略通知，继续读
                            }

                            // 如果是响应（有 id），检查是否匹配
                            if (response.Id == request.Id)
                            {
                                GlobalLogger.Debug($"DOCKER REQUEST RESPONSE [ACK]");
                                GlobalLogger.Debug($"DOCKER REQUEST RESPONSE ${response}");
                                return response;  // ← 找到了对应的响应
                            }
                        }
                    }
                    catch (OperationCanceledException e)
                    {
                        GlobalLogger.Debug($"DOCKER RESPONSE TIMEOUT AFTER {timeout}s.");
                        await EnsureGatewayRunningAsync();
                        await SessionInitializeAsync();
                        _stdin.WriteLine(json);
                        await _stdin.FlushAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalLogger.Debug($"[✗] 请求失败: {ex.Message}");
                return null;
            }
        }

        private async Task EnsureGatewayRunningAsync()
        {
            // 如果进程不存在或已退出，重新启动
            //if (_process == null || _process.HasExited)
            {
                GlobalLogger.Debug("[*] 重新启动 Gateway...");

                // 清理旧进程

                _stdin?.Dispose();
                _stdout?.Dispose();
                _process?.Kill();
                _process?.Dispose();

                var ret = await InitializeAsync();
                if (ret) { }
            }
        }

        private int GetNextRequestId()
        {
            return ++_requestId;
        }

        public void Dispose()
        {
            _stdin?.Dispose();
            _stdout?.Dispose();
            _process?.Kill();
            _process?.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            this.Dispose();
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>
    /// MCP 请求格式
    /// </summary>
    public class MCPRequest
    {
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; } = "2.0";

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        [JsonPropertyName("params")]
        public object Params { get; set; }
    }

    /// <summary>
    /// MCP 响应格式
    /// </summary>
    public class MCPResponse
    {
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("result")]
        public object Result { get; set; }

        [JsonPropertyName("error")]
        public MCPError Error { get; set; }

        [JsonPropertyName("method")]
        public string Method { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    /// <summary>
    /// MCP 错误格式
    /// </summary>
    public class MCPError
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    /// <summary>
    /// 工具列表
    /// </summary>
    public class ToolList
    {
        [JsonPropertyName("tools")]
        public Tool[] Tools { get; set; }
    }

    /// <summary>
    /// 工具定义
    /// </summary>
    public class Tool
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("inputSchema")]
        public object InputSchema { get; set; }
    }



}
