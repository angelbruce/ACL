using System;
using System.Linq;
using ABL.Extends;
using System.IO;
using System.Xml.Serialization;
using ABL.Object;
using System.Runtime.CompilerServices;
namespace ABL
{
    public static class ObjectExtend
    {
        public static bool IsDigit(this string str)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str, @"^[+-]?(\d+\.?\d*)|(\d*\.?\d+)$");
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Clone<T>(this object obj)
        {
            if (obj == null) return default(T);
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer x = new XmlSerializer(obj.GetType()); x.Serialize(stream, obj);
                byte[] content = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(content, 0, content.Length);
                stream.Flush(); stream.Position = 0; return x.Deserialize(stream).As<T>();
            }
        }

        /// <summary>
        /// 强制转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T As<T>(this object obj)
        {
            if (obj == null) return default(T);
            try
            {
                return (T)obj;
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// 多类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ParseTo<T>(this object obj)
        {
            if (obj == null) return default(T);
            Type sourceType = obj.GetType();
            Type valueType = typeof(T);
            if (obj.GetType() == valueType) return (T)obj;
            if (typeof(Enum).IsAssignableFrom(valueType))
                return (T)Enum.Parse(valueType, obj.ToString());
            if (valueType.IsAssignableFrom(obj.GetType())) return (T)obj;
            return BasicParseContext.Parse<T>(obj);
        }

        /// <summary>
        /// 多类型转换
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ParseTo(this object obj, Type type)
        {
            if (obj == null) return null;
            Type sourceType = obj.GetType();
            Type valueType = type;
            if (obj.GetType() == valueType) return obj;
            if (typeof(Enum).IsAssignableFrom(valueType))
                return Enum.Parse(valueType, obj.ToString());
            if (valueType.IsAssignableFrom(obj.GetType())) return obj;
            return BasicParseContext.Parse(obj, type);
        }

        /// <summary>
        /// 尝试转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T TryParseTo<T>(this object obj)
        {
            try
            {
                return ParseTo<T>(obj);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// 设置属性值
        /// 建议使用dynamic
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void Set(this object instance, string property, object value)
        {
            if (instance == null || string.IsNullOrEmpty(property)) return;
            var set = from d in instance.GetType().GetProperties() where (!d.IsSpecialName) && d.Name == property select d;
            if (set.Count() <= 0) return;
            System.Reflection.PropertyInfo prop = set.First();
            if (value == null || prop.PropertyType == value.GetType())
            {
                prop.SetValue(instance, value, new object[0]);
                return;
            }
            prop.SetValue(instance, value.ParseTo(prop.PropertyType), new object[0]);
        }

        /// <summary>
        /// 设置属性值
        /// 建议使用dynamic
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="property"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public static void Set(this object instance, string property, int index, object value)
        {
            if (index < 0) return; if (instance == null || string.IsNullOrEmpty(property)) return;
            var set = from d in instance.GetType().GetProperties() where (!d.IsSpecialName) && d.Name == property select d;
            if (set.Count() <= 0) return;
            System.Reflection.PropertyInfo prop = set.First();
            if (value == null || prop.PropertyType == value.GetType())
            {
                prop.SetValue(instance, value, new object[index]);
                return;
            }
            prop.SetValue(instance, value.ParseTo(prop.PropertyType), new object[index]);
        }

        /// <summary>
        /// 设置属性值
        /// 建议使用dynamic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        public static void Set<T>(this object instance, T value)
        {
            if (instance == null) return;
            var set = from d in instance.GetType().GetProperties() where (!d.IsSpecialName) && d.DeclaringType == typeof(T) select d;
            if (set.Count() <= 0) return;
            System.Reflection.PropertyInfo prop = set.First();
            prop.SetValue(instance, value, new object[0]);
        }

        /// <summary>
        /// 设置属性值
        /// 建议使用dynamic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public static void Set<T>(this object instance, int index, T value)
        {
            if (index < 0) return;
            if (instance == null) return;
            var set = from d in instance.GetType().GetProperties() where (!d.IsSpecialName) && d.DeclaringType == typeof(T) select d;
            if (set.Count() <= 0) return;
            System.Reflection.PropertyInfo prop = set.First();
            prop.SetValue(instance, value, new object[index]);
        }

        /// <summary>
        /// 获取属性值
        /// 建议使用dynamic
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static object Inner(this object instance, string property)
        {
            if (instance == null || string.IsNullOrEmpty(property)) return null;
            System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
            System.Reflection.PropertyInfo prop = instance.GetType().GetProperty(property);
            if (prop == null) prop = instance.GetType().GetProperty(property, flags);
            if (prop != null) return prop.GetValue(instance, new object[0]);
            System.Reflection.FieldInfo field = instance.GetType().GetField(property);
            if (field == null) field = instance.GetType().GetField(property, flags);
            if (field != null) return field.GetValue(instance); return null;
        }

        /// <summary>
        /// 获取属性值
        /// 建议使用dynamic
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="property"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static object Inner(this object instance, string property, int index)
        {
            if (index < 0) return null; if (instance == null || string.IsNullOrEmpty(property)) return null;
            System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
            System.Reflection.PropertyInfo prop = instance.GetType().GetProperty(property); if (prop == null) prop = instance.GetType().GetProperty(property, flags);
            if (prop != null) return prop.GetValue(instance, new object[index]);
            System.Reflection.FieldInfo field = instance.GetType().GetField(property);
            if (field == null) field = instance.GetType().GetField(property, flags);
            if (field != null) return field.GetValue(instance); return null;
        }

        /// <summary>
        /// 获取属性值
        /// 建议使用dynamic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static T Inner<T>(this object instance, string property)
        {
            return Inner(instance, property).As<T>();
        }

        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static System.Reflection.FieldInfo RefField(this object obj, string name)
        {
            if (obj == null) return null;
            var field = obj.GetType().GetField(name);
            if (field == null) obj.GetType().GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return field;
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static System.Reflection.PropertyInfo RefProperty(this object obj, string name)
        {
            if (obj == null) return null;
            var prop = obj.GetType().GetProperty(name);
            if (prop == null) prop = obj.GetType().GetProperty(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return prop;
        }

        /// <summary>
        /// 获取事件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static System.Reflection.EventInfo RefEvent(this object obj, string name)
        {
            if (obj == null) return null;
            var evt = obj.GetType().GetEvent(name);
            if (evt == null) evt = obj.GetType().GetEvent(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return evt;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static System.Reflection.MethodInfo RefMethod(this object obj, string name)
        {
            if (obj == null) return null;
            var evt = obj.GetType().GetMethod(name);
            if (evt == null) evt = obj.GetType().GetMethod(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return evt;
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static System.Reflection.MethodInfo RefMethod(this object obj, string name, Type[] types)
        {
            if (obj == null) return null;
            return obj.GetType().GetMethod(name, types);
        }

        /// <summary>
        /// 获取成员
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static System.Reflection.MemberInfo[] RefMember(this object obj, string name)
        {
            if (obj == null) return null;
            var member = obj.GetType().GetMember(name);
            return member == null ? obj.GetType().GetMember(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) : null;
        }

        /// <summary>
        /// 获取成员
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="memberTypes"></param>
        /// <returns></returns>
        public static System.Reflection.MemberInfo[] RefMember(this object obj, string name, System.Reflection.MemberTypes memberTypes)
        {
            if (obj == null) return null;
            var member = obj.GetType().GetMember(name, memberTypes, System.Reflection.BindingFlags.Default);
            return member == null ? obj.GetType().GetMember(name, memberTypes, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) : null;
        }

        /// <summary>
        /// 获取字段或属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static System.Reflection.MemberInfo RefFieldOrProperty(this object obj, string name)
        {
            if (obj == null) return null;
            var o = obj.RefProperty(name);
            if (o != null) return o;
            return obj.RefField(name);
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object Invoke(this object obj, string method, params object[] parameters)
        {
            var methodInfo = obj.RefMethod(method);
            return methodInfo.Invoke(obj, parameters);
        }


        /// <summary>
        /// copy from one data to another
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="obj"></param>
        /// <param name="target"></param>
        /// <param name="ignoreCase"></param>
        public static void CopyTo<T, V>(this T obj, V target, bool ignoreCase = false)
           where T : AbstractData
           where V : AbstractData
        {
            var vals = new Dictionary<string, object>();
            foreach (var prop in obj.GetType().GetProperties())
            {
                if (prop.IsSpecialName) continue;
                vals.Add(ignoreCase ? prop.Name.ToLower() : prop.Name, prop.GetValue(obj));
            }

            foreach (var prop in target.GetType().GetProperties())
            {
                if (prop.IsSpecialName) continue;
                if (!prop.CanWrite) continue;
                if (ignoreCase && !vals.ContainsKey(prop.Name.ToLower())) continue;
                else if (!ignoreCase && !vals.ContainsKey(prop.Name)) continue;

                var val = ignoreCase ? vals[prop.Name.ToLower()] : vals[prop.Name];
                var valType = prop.PropertyType;
                prop.SetValue(target, val.ParseTo(valType));

            }
        }
    }
}