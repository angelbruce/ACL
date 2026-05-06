using ACL.business.log;
using ACL.meta;
using ModelContextProtocol.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace ACL.business.mcp.dialect
{
    [MCPFactory]
    internal class StdMCP : IMCPable
    {
        private List<StdioMcpInfo>? mcpServers;
        private Dictionary<string, McpClient>? clients;
        private const string MCP_PREFIX = "__STDIO__";
        private Dictionary<string, HashSet<string>> passedTools;
        private Dictionary<string, MCPTool> allTools;

        public StdMCP()
        {
            passedTools = new Dictionary<string, HashSet<string>>();
            allTools = new Dictionary<string, MCPTool>();
        }

        public bool Test(string fnName)
        {
            if (fnName == null || fnName.Length == 0)
            {
                return false;
            }

            return fnName.StartsWith(MCP_PREFIX);
        }



        public async Task<MCPToolCallResult> CallToolAsync(string toolName, object arguments)
        {
            GlobalLogger.Debug($"工具调用{toolName}({arguments})");
            var mcpTool = allTools.ContainsKey(toolName) ? allTools[toolName] : null;
            if (mcpTool == null)
            {
                GlobalLogger.Error($"工具{toolName}未找到");
                return new MCPToolCallResult
                {
                    Success = false,
                    Content = null,
                    Error = $"工具{toolName}未找到",
                };
            }

            toolName = toolName.Substring(MCP_PREFIX.Length + 1);
            var idx = toolName.IndexOf("-");
            if (idx == -1)
            {
                return new MCPToolCallResult
                {
                    Success = false,
                    Content = null,
                    Error = $"invalid tool name {toolName}",
                };
            }

            var serverName = toolName.Substring(0, idx);
            var servers = await GetMcpServers();
            if (!servers.TryGetValue(serverName, out var client))
            {
                return new MCPToolCallResult
                {
                    Success = false,
                    Content = null,
                    Error = $"server {serverName} not found",
                };
            }


            toolName = toolName.Substring(idx + 1);
            var ags = JsonSerializer.Deserialize<Dictionary<string, object>>(arguments.ToString());
            while (true)
            {
                try
                {
                    using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
                    {
                        var callResult = await client.CallToolAsync(toolName, ags, cancellationToken: cts.Token);
                        var content = callResult.StructuredContent?.ToString();
                        if (content == null)
                        {
                            var sbd = new StringBuilder();
                            var blocks = callResult.Content;
                            foreach (var block in blocks)
                            {
                                sbd.Append(block.ToString());
                            }

                            content = sbd.ToString();
                        }
                        var success = callResult.IsError == null ? true : (bool)callResult.IsError;
                        GlobalLogger.Debug($"工具调用{toolName}完成，结果: {content}");
                        var patched = mcpTool.OutputSchema.ToString() == string.Empty ? "" : $"重要的补充说明，返回结果的结构如下：{mcpTool.OutputSchema.ToString()}，请提取需要的信息。";
                        return new MCPToolCallResult
                        {
                            Success = success,
                            Content = $"{content}。{patched}",
                            Error = success ? null : content,
                        };
                    }

                }
                catch (OperationCanceledException ex)
                {

                }
                catch (Exception e)
                {
                    return new MCPToolCallResult
                    {
                        Success = false,
                        Content = null,
                        Error = $"调用工具{toolName}失败: {e.Message}",
                    };
                }
            }
        }

        public ValueTask DisposeAsync()
        {
            mcpServers?.Clear();
            mcpServers = null;
            if (clients != null)
            {
                foreach (var item in clients)
                {
                    item.Value.DisposeAsync();
                }
            }

            clients?.Clear();
            clients = null;

            return ValueTask.CompletedTask;
        }

        public async Task<bool> InitializeAsync()
        {
            return await Task.FromResult(true);
        }

        public async Task<List<MCPTool>> ListToolsAsync()
        {
            var tools = new List<MCPTool>();
            var servers = await GetMcpServers();
            GlobalLogger.Debug($"获取所有MCP服务器的工具列表，共{servers.Count}个服务器");
            foreach (var item in servers)
            {
                GlobalLogger.Debug($"获取MCP服务器{item.Key}的工具列表");
                var name = item.Key;
                var server = item.Value;
                var serverTools = await server.ListToolsAsync();
                var passed = passedTools.ContainsKey(name) ? passedTools[name] : null;
                foreach (var tool in serverTools)
                {
                    if (passed != null && passed.Contains(tool.Name))
                    {
                        GlobalLogger.Info($"工具{tool.Name}在服务器{name}中被标记为已跳过");
                        continue;
                    }

                    GlobalLogger.Debug($"{tool.JsonSchema.ToString()}");

                    var mcpTool = new MCPTool
                    {
                        Name = $"{MCP_PREFIX}-{name}-{tool.Name}",
                        Description = tool.Description,
                        InputSchema = BinaryData.FromString(tool.JsonSchema.ToString()),
                        OutputSchema = BinaryData.FromString(tool.ReturnJsonSchema == null ? string.Empty : tool.ReturnJsonSchema?.ToString()),
                    };

                    tools.Add(mcpTool);
                    allTools.Add(mcpTool.Name, mcpTool);
                }
                GlobalLogger.Debug($"获取MCP服务器{item.Key}的工具列表完成");
            }

            GlobalLogger.Debug($"获取所有MCP服务器的工具列表完成，共{tools.Count}个工具");
            return await Task.FromResult(tools);
        }

        public async Task<bool> SessionInitializeAsync()
        {
            return await Task.FromResult(true);
        }

        private async Task<Dictionary<string, McpClient>> GetMcpServers()
        {
            if (clients != null)
            {
                return clients;
            }

            if (mcpServers == null)
            {
                mcpServers = await StdioMcpConfig.GetStdioMcpInfoAsync();
            }

            if (mcpServers == null)
            {
                return new Dictionary<string, McpClient>();
            }

            var mcpClients = new Dictionary<string, McpClient>();
            foreach (var server in mcpServers)
            {
                if (server == null) continue;
                if (server.Name == null || server.Name.Length == 0) continue;

                if (server.Command == null || server.Command.Length == 0) continue;
                GlobalLogger.Info($"Starting MCP server {server.Name} with command {server.Command} and args {string.Join(",", server.Args?.Select(arg => arg.Name.ToString())?.ToList() ?? [])}");
                if (server.PassedTools != null)
                {
                    passedTools.Add(server.Name, new HashSet<string>());
                    foreach (var tool in server.PassedTools)
                    {
                        passedTools[server.Name].Add(tool.Name);
                    }
                }

                var transport = new StdioClientTransport(new StdioClientTransportOptions
                {
                    Name = server.Name,
                    Command = server.Command,
                    Arguments = server.Args?.Select(arg => arg.Name.ToString())?.ToList() ?? [],
                });

                try
                {
                    var client = await McpClient.CreateAsync(transport);
                    mcpClients.Add(server.Name, client);
                }
                catch (Exception ex)
                {
                    GlobalLogger.Error($"Failed to start MCP server {server.Name} with command {server.Command}: {ex.Message}");
                }
            }

            clients = mcpClients;
            return clients;
        }
    }
}
