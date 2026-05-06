using System;
using System.Collections.Generic;
using ABL.Object;
namespace ABL.Enums
{
    public class EnumHelper
    {
        static EnumHelper helper = null;
        static object dummy = new object();
        Dictionary<Type, Dictionary<Enum, string>> containerEnums = null;
        Dictionary<Type, List<string>> containerItems = null;
        Type descType = typeof(DescriptionAttribute);

        public static EnumHelper Helper
        {
            get
            {
                if (helper == null)
                {
                    lock (dummy)
                    {
                        if (helper == null)
                        {
                            helper = new EnumHelper();
                        }
                    }
                }
                return helper;
            }
        }

        private EnumHelper()
        {
            containerEnums = new Dictionary<Type, Dictionary<Enum, string>>();
            containerItems = new Dictionary<Type, List<string>>();
        }

        private void Register(Enum e)
        {
            Type type = e.GetType();
            if (containerEnums.ContainsKey(type)) return;
            lock (dummy)
            {
                if (!containerEnums.ContainsKey(type))
                {
                    Dictionary<Enum, string> enumContainer = new Dictionary<Enum, string>();
                    containerEnums.Add(type, enumContainer);
                    List<string> items = new List<string>();
                    containerItems.Add(type, items);
                    System.Reflection.FieldInfo[] fields = type.GetFields();
                    foreach (System.Reflection.FieldInfo field in fields)
                    {
                        if (field.IsSpecialName) continue;
                        object[] attributes = field.GetCustomAttributes(false);
                        foreach (object o in attributes)
                        {
                            DescriptionAttribute? attribute = o as DescriptionAttribute;
                            if (attribute == null) continue;
                            try
                            {
                                string s = attribute.Name;
                                Enum value = (Enum)field.GetValue(e);
                                enumContainer.Add(value, s);
                                items.Add(s); break;
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        private void Register(Type type)
        {
            if (type == null || !type.IsEnum) return;
            object? o = Activator.CreateInstance(type);
            if (o == null) return;
            Enum e = o as Enum;
            if (e == null) return;
            Register(e);
        }

        public string this[Enum e]
        {
            get
            {
                Register(e);
                Dictionary<Enum, string> enumContainer = containerEnums[e.GetType()];
                if (enumContainer == null || !enumContainer.ContainsKey(e)) return null;
                return enumContainer[e];
            }
        }

        public string Description(Enum e)
        {
            Register(e);
            Dictionary<Enum, string> enumContainer = containerEnums[e.GetType()];
            if (enumContainer == null || !enumContainer.ContainsKey(e)) return null;
            return enumContainer[e];
        }

        public List<string> Strings(Type type)
        {
            if (type == null || !type.IsEnum) return null;
            if (!containerEnums.ContainsKey(type)) Register(type); return containerItems[type];
        }

        public List<Item> Items(Type type)
        {
            if (type == null || !type.IsEnum) return null;
            if (!containerEnums.ContainsKey(type)) Register(type);
            Dictionary<Enum, string> dictionary = containerEnums[type];
            List<Item> list = new List<Item>();
            foreach (KeyValuePair<Enum, string> pair in dictionary)
            {
                Item item = new Item();
                item.Id = pair.Key.ToString();
                item.Name = pair.Value; list.Add(item);
            }
            return list;
        }

        public T ParseDesc<T>(string desc)
        {
            Type type = typeof(T);
            if (!type.IsEnum) throw new Exception("类型T不是枚举类型");
            Register(type);
            Dictionary<Enum, string> enums = containerEnums[type];
            foreach (KeyValuePair<Enum, string> pair in enums)
            {
                if (pair.Value == desc)
                {
                    object o = pair.Key; return (T)o;
                }
            }
            throw new Exception(string.Format("没有为枚举类型{0}提供注释{1}", type.FullName, desc));
        }

        public T Parse<T>(string str)
        {
            Type type = typeof(T);
            if (!type.IsEnum) throw new Exception("类型T不是枚举类型");
            Register(type);
            return (T)Enum.Parse(type, str);
        }

        public T Parse<T>(string str, bool ignoreCase)
        {
            if (!ignoreCase) return Parse<T>(str);
            Type type = typeof(T);
            if (!type.IsEnum) throw new Exception("类型T不是枚举类型");
            Register(type);
            return (T)Enum.Parse(type, str, true);
        }

        public T ParseDesc<T>(string desc, bool ignoreCase)
        {
            if (!ignoreCase) return ParseDesc<T>(desc);
            Type type = typeof(T);
            if (!type.IsEnum) throw new Exception("类型T不是枚举类型");
            Register(type);
            Dictionary<Enum, string> enums = containerEnums[type];
            foreach (KeyValuePair<Enum, string> pair in enums)
            {
                if (pair.Value.ToLower() == desc.ToLower())
                {
                    object o = pair.Key;
                    return (T)o;
                }
            } 
            throw new Exception(string.Format("没有为枚举类型{0}提供注释{1}", type.FullName, desc));
        }

    }
}