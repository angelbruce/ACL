using ABL;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using static System.Windows.Forms.DataFormats;

namespace ACL.business.mcp
{
    public class MCPTool
    {
        public string Name { get; set; }
        public string Description { get; set; }


        [Field(Formator = typeof(BionaryDataFormat))]
        public BinaryData InputSchema { get; set; }


        [Field(Formator = typeof(BionaryDataFormat))]
        public BinaryData OutputSchema { get; set; }
    }

    public class BionaryDataFormat : DefaultFormat
    {
        public object Format(Type inType, object value, string format, Type outType)
        {

            if (outType == typeof(BinaryData) && inType == typeof(string))
            {
                return BinaryData.FromString(value.ParseTo<string>());
            }

            return base.Format(inType, value, format, outType);
        }
    }

    /// <summary>
    /// 工具调用结果
    /// </summary>
    public class MCPToolCallResult
    {
        public bool Success { get; set; }
        public object Content { get; set; }
        public string Error { get; set; }
    }
}
