using System.Collections;
using System.Text;

namespace ABL.Object
{

    /***
     *  
     *  文档说明
     *  提供灵活的JSON结构定义用来支持需要灵活定义的场景，而非基于固定结构的场景，例如：存在多层嵌套，结构过于复杂。
     *  
    */


    /// <summary>
    /// JSON 接口
    /// </summary>
    public interface IJsonWriter
    {
        /// <summary>
        /// 输出JSON字符串
        /// </summary>
        /// <returns></returns>
        string GetJson();
    }

    /// <summary>
    /// JSON数组
    /// </summary>
    public class JArray : IJsonWriter, IEnumerable<IJsonWriter>
    {
        protected List<IJsonWriter> datas = new List<IJsonWriter>();
        private StringBuilder sbd = new StringBuilder();


        public JArray() { }
        /// <summary>
        /// 包含的元素个数
        /// </summary>
        public int Count
        {
            get
            {
                return datas.Count;
            }
        }

        public IEnumerator<IJsonWriter> GetEnumerator()
        {
            return datas.GetEnumerator();
        }

        /// <summary>
        /// 输出JSON字符串[]
        /// </summary>
        /// <returns></returns>
        public virtual string GetJson()
        {
            if (sbd.Length == 0)
            {
                sbd.Append('[');
                foreach (var item in datas)
                {
                    sbd.Append(item.GetJson());
                    sbd.Append(',');
                }

                if (datas.Count > 0) sbd.Length--;
                sbd.Append(']');
            }

            return sbd.ToString();
        }

        public void Write(string data)
        {
            if (data == null)
            {
                datas.Add(new JNull());
            }
            else
            {
                var jvalue = new JValue(data);
                datas.Add(jvalue);
            }
        }

        /// <summary>
        /// 写入一个元素到数组中
        /// </summary>
        /// <param name="data"></param>
        public void Write(object data)
        {
            if (data == null)
            {
                datas.Add(new JNull());
            }
            else if (data is IJsonWriter ij)
            {
                datas.Add(ij);
            }
            else
            {
                var jvalue = new JValue(data);
                datas.Add(jvalue);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// JSON对象object
    /// </summary>
    public class JObject : IJsonWriter
    {
        protected Dictionary<string, IJsonWriter> datas = new Dictionary<string, IJsonWriter>();
        private StringBuilder sbd = new StringBuilder();

        public JObject() { }

        /// <summary>
        /// 包含的元素个数
        /// </summary>
        public int Count
        {
            get { return datas.Count; }
        }

        /// <summary>
        /// 是否包含某个键
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return datas.ContainsKey(name);
        }

        /// <summary>
        /// get the value of name from datas in the json object.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IJsonWriter? Get(string name)
        {
            if (datas.ContainsKey(name)) return datas[name];
            return null;
        }

        /// <summary>
        /// 输出JSON字符串{}
        /// </summary>
        /// <returns></returns>
        public virtual string GetJson()
        {
            if (sbd.Length == 0)
            {
                sbd.Append('{');
                foreach (var item in datas)
                {
                    sbd.Append($"\"{item.Key}\":{item.Value.GetJson()},");
                }

                if (datas.Count > 0) sbd.Length--;

                sbd.Append('}');
            }

            return sbd.ToString();
        }

        /// <summary>
        /// 写入一个对象
        /// </summary>
        /// <param name="data"></param>
        public void Write(object data)
        {
            if (data == null) return;

            var type = data.GetType();
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                var pval = prop.GetValue(data, null);
                if (pval == null)
                {
                    continue;
                }

                var vtype = prop.DeclaringType;
                if (vtype == null)
                {
                    continue;
                }

                var jval = new JValue(pval);

                datas[prop.Name] = jval;
            }
        }
    }

    /// <summary>
    /// json number
    /// </summary>
    public class JNumeric : IJsonWriter
    {
        string data = string.Empty;
        public JNumeric(object d)
        {
            if (d == null)
            {
                data = "null";
                return;
            }

            data = d.ToString() ?? string.Empty;
        }
        public string GetJson()
        {
            return data;
        }

        public float Value
        {
            get
            {
                if (data == null || data == "null") return float.NaN;

                return float.Parse(data);
            }
        }
    }

    /// <summary>
    /// json bool
    /// </summary>
    public class JBool : IJsonWriter
    {
        string data = string.Empty;
        public JBool(object val)
        {
            if (val == null)
            {
                data = "false";
                return;
            }

            bool? v = (bool)val;
            if (v == null) data = "false";
            else
            {
                data = ((bool)val) ? "true" : "false";
            }
        }

        public string GetJson()
        {
            return data;
        }

        public bool Value
        {
            get
            {
                if (data == null) return false;
                return data == "true";
            }
        }
    }

    /// <summary>
    /// joson null
    /// </summary>
    public class JNull : IJsonWriter
    {
        public string GetJson()
        {
            return "null";
        }
    }

    /// <summary>
    /// json string
    /// </summary>
    public class JStr : IJsonWriter
    {
        public string val;

        public JStr() { val = "null"; }

        public JStr(object data)
        {
            if (data == null) val = "null";
            else val = $"\"{data.ParseTo<string>()}\"";
        }

        public string GetJson()
        {
            return val;
        }

        public string Value
        {
            get
            {
                if (string.IsNullOrEmpty(val)) return null;
                if (val == "null") return null;
                return val.Substring(1, val.Length - 2);
            }
        }
    }

    /// <summary>
    /// convert a value to ijson which type can be JArray/JObject/JBool/JNumeric/JStr/JNull
    /// </summary>
    public class JValue : IJsonWriter
    {
        protected IJsonWriter? ijson = null;
        string? json = null;

        public static string GetJsonDataType(Type type)
        {
            if (type.IsEnum)
            {
                return "string";
            }

            if (type == typeof(string)
                || type == typeof(StringBuilder)
                || type == typeof(DateTime)
                ) return "string";

            else if (type == typeof(int)
                       || type == typeof(uint)
                       || type == typeof(byte)
                       || type == typeof(short)
                       || type == typeof(float)
                       || type == typeof(double)
                       || type == typeof(decimal)
                       ) return "number";
            else if (type == typeof(bool)) return "boolean";
            else if (typeof(IEnumerable).IsAssignableFrom(type)) return "array";
            else return "object";
        }

        public virtual string GetJson()
        {
            if (ijson == null)
            {
                return null;
            }

            if (json == null)
            {
                json = ijson.GetJson();
            }

            return json;
        }

        /// <summary>
        /// 将传入的data转换为必要的IJson具体对象：JArray/JObject/JBool/JNumeric/JStr/JNull
        /// </summary>
        /// <param name="data"></param>
        public JValue(object data)
        {
            IJsonWriter? j = null;
            if (data == null)
            {
                j = new JNull();
            }
            else
            {
                var type = data.GetType();
                if (type == typeof(string)
                    || type == typeof(StringBuilder)
                    || type == typeof(DateTime)
                    )
                {
                    j = new JStr(data);
                }
                else if (type.IsPrimitive)
                {
                    if (type == typeof(int)
                        || type == typeof(uint)
                        || type == typeof(byte)
                        || type == typeof(short)
                        || type == typeof(float)
                        || type == typeof(double)
                        || type == typeof(decimal)
                        || type == typeof(DateTime)
                        )
                    {
                        j = new JNumeric(data);
                    }
                    else if (type == typeof(bool))
                    {
                        j = new JBool(data);
                    }
                }
                else if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    j = new JArray();
                    var array = new JArray();
                    var enumerator = ((IEnumerable)data).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        array.Write(enumerator.Current);
                    }

                    j = array;
                    return;

                }
            }

            ijson = j;
        }

    }

    /// <summary>
    /// 便捷的json object，支持直接写入key、value
    /// </summary>
    public class JsonObject : JObject
    {
        public void Write(string name, object value)
        {
            if (value == null) return;

            if (value is IJsonWriter ij)
            {
                datas[name] = ij;
                return;
            }

            datas[name] = new JValue(value);
        }

    }

    /// <summary>
    /// 便捷的json array，只支持写入集合
    /// </summary>
    public class JsonArray : JArray
    {
        private bool isNull;

        public JsonArray() { }

        public override string GetJson()
        {
            if (isNull)
            {
                return "null";
            }
            else
            {
                return base.GetJson();
            }
        }
    }


}
