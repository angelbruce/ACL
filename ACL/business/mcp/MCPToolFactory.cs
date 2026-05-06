using ACL.business.log;
using ACL.business.mcp.dialect;
using McpOrchestrator;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ACL.business.mcp
{
    public class MCPToolFactory : IAsyncDisposable
    {
        private List<IMCPable>? mcpInstances;

        public async Task<List<MCPTool>> GetToolsAsync()
        {
            var mcpFactories = await CreateInstances();

            var list = new List<MCPTool>();

            foreach (var item in mcpFactories)
            {
                var session = await item.SessionInitializeAsync();
                if (session)
                {
                    var mcpTools = await item.ListToolsAsync();
                    if (mcpTools != null)
                    {
                        list.AddRange(mcpTools);
                    }
                }
            }


            return list;
        }


        public async Task<bool> InitSessionAsync()
        {
            var mcpFactories = await CreateInstances();
            foreach (var item in mcpFactories)
            {
                await item.SessionInitializeAsync();
            }

            return true;
        }

        public async Task<MCPToolCallResult> CallToolAsync(string name, BinaryData args)
        {
            var mcpFactories = await CreateInstances();
            foreach (var item in mcpFactories)
            {
                if (item.Test(name))
                {
                    return await item.CallToolAsync(name, args);
                }
            }

            return new MCPToolCallResult() { Success = false, Error = "未知的错误，调用发生了异常！" };
        }

        private async Task<List<IMCPable>> CreateInstances()
        {
            if (mcpInstances == null)
            {
                var mcpFactories = await GetMCPFactoryInstances();
                var datas = mcpFactories;
                this.mcpInstances = mcpFactories;
            }

            return mcpInstances;
        }


        private async Task<List<IMCPable>> GetMCPFactoryInstances()
        {
            var list = new List<IMCPable>();
            var type = typeof(meta.MCPFactoryAttribute);
            var types = type.Assembly.GetTypes();
            var set = new HashSet<string>();
            foreach (var item in types)
            {
                if (!item.IsClass) continue;
                if (item.IsInterface) continue;
                if (!typeof(IMCPable).IsAssignableFrom(item.DeclaringType)) continue;

                var instType = item.DeclaringType;

                var attr = instType.GetCustomAttribute(typeof(meta.MCPFactoryAttribute));
                if (attr == null) continue;

                var fullname = instType.FullName;
                if (fullname == null) continue;

                if (set.Contains(fullname)) continue;
                set.Add(fullname);

                var consturctor = instType.GetConstructor([]);
                if (consturctor == null) continue;

                IMCPable inst = (IMCPable)consturctor.Invoke([]);
                if (inst != null)
                {
                    await inst.InitializeAsync();
                    list.Add(inst);
                }

            }

            return await Task.FromResult(list);
        }

        public async ValueTask DisposeAsync()
        {
            if (mcpInstances != null)
            {
                foreach (var item in mcpInstances)
                {
                    await item.DisposeAsync();
                }

                mcpInstances = null;
            }
        }
    }

}
