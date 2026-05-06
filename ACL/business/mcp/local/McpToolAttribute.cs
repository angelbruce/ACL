using ABL.Object;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace ACL.business.mcp.local
{
    /// <summary>
    /// 指示一个类中包含MCP的TOOL
    /// </summary>
    public class McpServerToolAttribute : Attribute
    {
    }

    /// <summary>
    /// 指示方法为MCP工具
    /// </summary>
    public class McpToolAttribute : Attribute
    {
    }

    /// <summary>
    /// 指示哪些属性或参数是必填的
    /// </summary>
    public class RequiredAttribute : Attribute
    {
    }

    /// <summary>
    /// MCP 加载异常
    /// </summary>
    public class McpException : Exception
    {
        public McpException() { }
    }

    public class LocalMcpTool
    {
        public MCPTool MCPTool { get; set; }

        public MethodInfo Method { get; set; }
    }

    /// <summary>
    /// 从MCP类中加载tools的工具类
    /// </summary>
    public class McpServerTools
    {
        public McpServerTools()
        {
        }

        /// <summary>
        /// 加载当前项目中所有的TOOL
        /// </summary>
        public List<LocalMcpTool> LoadToolList()
        {
            var tools = new List<LocalMcpTool>();

            foreach (var type in typeof(McpServerTools).Assembly.GetTypes())
            {
                var mcpServerTool = type.GetCustomAttribute<McpServerToolAttribute>();
                if (mcpServerTool == null) continue;

                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                if (methods == null || methods.Length == 0) continue;

                foreach (var method in methods)
                {
                    var toolAttribute = method.GetCustomAttribute<McpToolAttribute>();
                    if (toolAttribute == null) continue;

                    var reslover = new McpToolReslover(method);
                    var mcpTool = reslover.ResloveTool();
                    if (mcpTool != null)
                    {
                        var localMcpTool = new LocalMcpTool();
                        localMcpTool.MCPTool = mcpTool;
                        localMcpTool.Method = method;
                        tools.Add(localMcpTool);

                    }
                }
            }

            return tools;
        }
    }

    /// <summary>
    /// MCP 工具解析
    /// </summary>
    public class McpToolReslover
    {
        /// <summary>
        /// 嵌套定义
        /// </summary>
        private JsonObject definitions;
        /// <summary>
        /// tool方法
        /// </summary>
        private MethodInfo method;

        /// <summary>
        /// 解析方法
        /// </summary>
        /// <param name="method"></param>
        public McpToolReslover(MethodInfo method)
        {
            definitions = new JsonObject();
            this.method = method;
        }

        /// <summary>
        /// 解析方法的MCPTool，严格遵守jsonrpc2.0
        /// </summary>
        /// <returns></returns>
        /// <exception cref="McpException"></exception>
        public MCPTool ResloveTool()
        {
            var sbd = new StringBuilder();
            var toolAttr = method.GetCustomAttribute<McpToolAttribute>();
            if (toolAttr == null)
            {
                throw new McpException();
            }

            var fn = new JsonObject();
            fn.Write("name", method.Name);

            string fnDesc = string.Empty;
            var fnDescAttr = method.GetCustomAttribute<DescriptionAttribute>();
            if (fnDescAttr != null)
            {
                fnDesc = fnDescAttr.Description;
            }
            fn.Write("description", fnDesc);

            var inputSchema = new JsonObject();
            inputSchema.Write("$schema", "http://json-schema.org/draft-07/schema#");
            inputSchema.Write("type", "object");
            inputSchema.Write("additionalProperties", false);

            var properties = new JsonObject();
            inputSchema.Write("properties", properties);

            var requireList = new JsonArray();

            var parameters = method.GetParameters();
            foreach (var param in parameters)
            {
                if (param.Name == null || param.Name.Length == 0) continue;

                var requiredAttr = param.GetCustomAttribute<RequiredAttribute>();
                if(requiredAttr != null)
                {
                    requireList.Write(param.Name);
                }


                var type = param.ParameterType;
                var descAttr = param.GetCustomAttribute<DescriptionAttribute>();
                var prop = new JsonObject();
                properties.Write(param.Name, prop);

                FillTypeInfo(type, prop, descAttr);
            }

            inputSchema.Write("required", requireList);

            if (definitions.Count > 0)
            {
                inputSchema.Write("definitions", definitions);
            }

            fn.Write("inputSchema", inputSchema);

            var paramsSchema = BinaryData.FromString(fn.GetJson());

            var tool = new MCPTool
            {
                Name = method.Name,
                Description = fnDesc,
                InputSchema = paramsSchema
            };

            return tool;
        }

        /// <summary>
        /// 类型定义 definitions部分
        /// </summary>
        /// <param name="type"></param>
        private void LoadDefinition(Type type)
        {
            if (type == null) return;

            var typeName = type.Name;
            if (definitions.Contains(typeName)) return;

            var typeDecl = new JsonObject();
            definitions.Write(type.Name, typeDecl);

            typeDecl.Write("type", "object");

            var props = new JsonObject();
            typeDecl.Write("properties", props);

            var requireList = new JsonArray();
            foreach (var prop in type.GetProperties())
            {
                var propType = prop.PropertyType;
                if (propType == null) continue;

                var requireAttr = prop.GetCustomAttribute<RequiredAttribute>();
                if (requireAttr != null) requireList.Write(prop.Name);

                var po = new JsonObject();
                props.Write(prop.Name, po);
                var descAttr = prop.GetCustomAttribute<DescriptionAttribute>();
                FillTypeInfo(propType, po, descAttr);
            }

            typeDecl.Write("required", requireList);
        }

        /// <summary>
        /// 类型定义 properties中的成员以及子成员
        /// </summary>
        /// <param name="type"></param>
        /// <param name="container"></param>
        private void FillTypeInfo(Type type, JsonObject container, DescriptionAttribute? desc)
        {
            var jtype = JValue.GetJsonDataType(type);
            container.Write("type", jtype);
            if (desc == null)
            {
                desc = type.GetCustomAttribute<DescriptionAttribute>();
            }

            if (desc != null)
            {
                container.Write("description", desc.Description);
            }

            switch (jtype)
            {
                case "string":
                    {
                        if (type.IsEnum)
                        {
                            var array = new JsonArray();
                            foreach (var field in type.GetFields())
                            {
                                if (!field.IsPublic || field.IsSpecialName) continue;

                                var name = field.Name;
                                array.Write(name);
                            }

                            if (array.Count > 0)
                            {
                                container.Write("enum", array);
                            }
                        }

                        break;
                    }
                case "object":
                    {
                        var items = new JsonObject();
                        container.Write("items", items);
                        items.Write("$ref", $"#/inputSchema/definitions/{type.Name}");
                        LoadDefinition(type);
                        break;
                    }
                case "array":
                    {
                        var innerType = type.GetGenericArguments().FirstOrDefault();
                        if (innerType == null) break;

                        var items = new JsonObject();
                        container.Write("items", items);
                        FillTypeInfo(innerType, items, null);
                        break;
                    }
            }

        }

    }


}