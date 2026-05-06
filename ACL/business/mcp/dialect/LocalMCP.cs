using ABL;
using ABL.Object;
using ACL.business.mcp.local;
using ACL.meta;
using System.Text.Json;

namespace ACL.business.mcp.dialect
{
    [MCPFactory]
    public class LocalMCP : IMCPable
    {
        private const string MCP_PREFIX = "__LOCAL__";

        private McpServerTools mcpServerTools;
        private List<MCPTool>? tools = null;
        private Dictionary<string, LocalMcpTool> toolMap;

        public LocalMCP()
        {
            mcpServerTools = new McpServerTools();
            toolMap = new Dictionary<string, LocalMcpTool>();
        }

        public async Task<MCPToolCallResult> CallToolAsync(string toolName, object arguments)
        {
            if (!toolMap.ContainsKey(toolName))
            {
                return new MCPToolCallResult() { };
            }

            if (arguments == null) arguments = "{}";

            var tool = toolMap[toolName];
            var method = tool.Method;
            var parameters = method.GetParameters();

            var reader = new JsonReader();
            var json = reader.ParseObject(arguments.ToString() ?? "{}");
            if (json == null) json = new JsonObject();

            var objs = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var name = parameter.Name;
                if (name == null) continue;

                if (json.Contains(name))
                {
                    var jval = json.Get(name).GetJson();
                    var value = JsonSerializer.Deserialize(json: jval, returnType: parameter.ParameterType);
                    if (value != null) objs[i] = value;
                }
            }

            MCPToolCallResult? mcpResult = null;
            try
            {
                Type returnType = method.ReturnType;
                bool isAsync = returnType == typeof(Task) || returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>);

                if (isAsync)
                {
                    object result = method.Invoke(null, objs);
                    try
                    {
                        if (result is Task task)
                        {
                            task.Wait();
                            var ret = result.Inner("Result");
                            mcpResult = new MCPToolCallResult()
                            {
                                Content = JsonSerializer.Serialize(ret),
                                Success = true
                            };
                        }
                    }
                    catch (AggregateException ex)
                    {
                        // 捕获任务内部的任何异常
                        throw ex.InnerException ?? ex;
                    }
                }
                else
                {
                    var ret = method.Invoke(null, objs);
                    mcpResult = new MCPToolCallResult()
                    {
                        Content = JsonSerializer.Serialize(ret),
                        Success = true
                    };
                }
            }
            catch (Exception ex)
            {
                mcpResult = new MCPToolCallResult()
                {
                    Error = ex.Message,
                    Success = false
                };
            }

            return mcpResult;
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public Task<bool> InitializeAsync()
        {
            return Task.FromResult(true);
        }

        public async Task<List<MCPTool>> ListToolsAsync()
        {
            if (tools == null)
            {
                var rawTools = mcpServerTools.LoadToolList();
                var list = new List<MCPTool>();
                foreach (var tool in rawTools)
                {
                    var name = $"{MCP_PREFIX}{tool.MCPTool.Name}";
                    tool.MCPTool.Name = name;
                    toolMap[name] = tool;
                    list.Add(tool.MCPTool);
                }

                tools = list;
            }

            return await Task.FromResult(tools);
        }

        public Task<bool> SessionInitializeAsync()
        {
            return Task.FromResult(true);
        }

        public bool Test(string prefix)
        {
            if (prefix == null || prefix.Length == 0)
            {
                return false;
            }

            return prefix.StartsWith(MCP_PREFIX);
        }
    }
}
