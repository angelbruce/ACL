using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ABL.Object
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property)]
    public class RJsonAttribute : Attribute
    {
        public string[] Names { get; set; } = [];

        public RJsonAttribute() { }
        public RJsonAttribute(params string[] names) { Names = names; }
    }

    public class PropJsonMeta
    {
        public PropertyInfo Property { get; set; }
        public string[] Names { get; set; }
    }

    public class RelexJSON
    {
        public JVal? TryParse(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;
            try
            {
                var reader = new JsonReader();
                var jval = reader.Parse(json);
                return jval;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<T> Deserealize<T>(JVal? jval) where T : class, new()
        {
            var list = new List<T>();
            if (jval == null) return list;

            var concretType = typeof(T);

            switch (jval.Type)
            {
                case JsonDataType.Array:
                    {
                        if (jval.Value is JsonArray vals)
                        {
                            var array = new List<T>();
                            FillArray(array, array.GetType(), vals);
                        }
                    }
                    break;
                case JsonDataType.Object:
                    if (jval.Value is JsonObject val)
                    {
                        var t = new T();
                        FillObject(t, concretType, val);
                        list.Add(t);
                    }

                    break;
            }

            return list;
        }

        public List<T> Deserealize<T>(string json) where T : class, new()
        {
            var reader = new JsonReader();
            var jval = reader.Parse(json);

            return Deserealize<T>(jval);
        }

        private void Fill(object? container, Type type, IJsonWriter jsonWriter)
        {
            switch (jsonWriter)
            {
                case JsonObject jsonObject:
                    {
                        FillObject(container, type, jsonObject);
                        break;
                    }
                case JsonArray array:
                    {
                        FillArray((IList)container, type, array);
                        break;
                    }
                case JStr jstr:
                    {
                        container = jstr.Value;
                        break;
                    }
                case JNull jnull:
                    {
                        container = null;
                        break;
                    }
                case JBool jbool:
                    {
                        container = jbool.Value;
                        break;
                    }
                case JNumeric jnumber:
                    {
                        container = jnumber.Value;
                        break;
                    }
            }
        }

        public void FillArray(IList containers, Type type, JsonArray array)
        {
            var concretType = type.GetGenericArguments()[0];
            foreach (var val in array)
            {
                if (val == null)
                {
                    containers.Add(default);
                    continue;
                }

                var data = Activator.CreateInstance(concretType);
                if (data != null)
                {
                    Fill(data, concretType, val);
                    containers.Add(data);
                }
            }
        }

        private void FillObject(object container, Type concretType, JsonObject jsonObject)
        {
            var rjsonMeta = FetchRJsons(concretType);
            if (rjsonMeta == null) return;

            foreach (var meta in rjsonMeta)
            {
                var prop = meta.Property;
                var names = meta.Names;
                foreach (var name in names)
                {
                    //找到1个有用的进行
                    var data = jsonObject.Get(name);
                    if (data == null) continue;

                    if (data is IJsonWriter pjsonVal)
                    {
                        var ptype = prop.PropertyType;
                        var pJsonMeta = FetchRJsons(ptype);
                        if (ptype == typeof(string))
                        {
                            if (data is JStr jstr)
                            {
                                var sval = jstr.Value;
                                if (sval == null) continue;

                                prop.SetValue(container, sval);

                                break;
                            }

                            continue;
                        }
                        else if (ptype == typeof(bool))
                        {
                            if (data is JBool jbool)
                            {
                                var jval = jbool.Value;
                                if (jval == null) continue;

                                prop.SetValue(container, jval);

                                break;
                            }

                            continue;
                        }
                        else if (
                            ptype == typeof(byte)
                            || ptype == typeof(short)
                            || ptype == typeof(int)
                            || ptype == typeof(long)
                            || ptype == typeof(float)
                            || ptype == typeof(double)
                            || ptype == typeof(decimal)
                            || ptype == typeof(ushort)
                            || ptype == typeof(uint)
                            || ptype == typeof(ulong)
                            )
                        {
                            if (data is JNumeric jnumber)
                            {
                                var jval = jnumber.Value;
                                if (jval == null) continue;

                                prop.SetValue(container, jval.ParseTo(ptype));

                                break;
                            }

                            continue;
                        }
                        var val = Activator.CreateInstance(ptype);
                        if (val != null)
                        {
                            Fill(val, ptype, pjsonVal);
                            prop.SetValue(container, val);
                        }
                    }
                    else
                    {
                        prop.SetValue(container, data);
                    }
                }
            }
        }


        private List<PropJsonMeta> FetchRJsons(Type type)
        {
            var datas = new List<PropJsonMeta>();
            foreach (var prop in type.GetProperties())
            {
                if (prop.IsSpecialName) continue;

                var attr = prop.GetCustomAttribute<RJsonAttribute>();
                if (attr == null) continue;

                datas.Add(new PropJsonMeta { Property = prop, Names = attr.Names });
            }

            return datas;
        }
    }
}
