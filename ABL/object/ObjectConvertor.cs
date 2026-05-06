using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace ABL
{
    /// <summary>
    /// 对象转换器
    /// </summary>
    public static class ObjectConvertor
    {

        /// <summary>
        /// 将DATATABLE数据集转换为实例链接集合
        /// </summary>
        /// <typeparam name="T">返回数据对象</typeparam>
        /// <param name="table">DATATABLE数据集</param>
        /// <returns></returns>
        public static List<T> ConvertList<T>(this DataTable table, Action<T> t) where T : new()
        {
            var list = new List<T>();
            if (table == null || table.Rows.Count == 0) return list;
            foreach (DataRow row in table.Rows)
            {
                var data = Convert<T>(row);
                if (data == null) continue;
                if (t != null) t(data);
                list.Add(data);
            }
            return list;
        }

        /// <summary>
        /// 将DATATABLE数据集转换为实例链接集合
        /// </summary>
        /// <typeparam name="T">返回数据对象</typeparam>
        /// <param name="table">DATATABLE数据集</param>
        /// <returns></returns>
        public static List<T> ConvertList<T>(this DataTable table) where T : new()
        {
            var list = new List<T>();
            if (table == null || table.Rows.Count == 0) return list;
            foreach (DataRow row in table.Rows)
            {
                var data = Convert<T>(row);
                if (data == null) continue;
                list.Add(data);
            }
            return list;
        }
        /// <summary>
        /// 将DATAROW转换为数据实例
        /// </summary>
        /// <typeparam name="T">返回的数据类型</typeparam>
        /// <param name="data">DATAROW数据</param>
        /// <returns></returns>
        public static T Convert<T>(DataRow data) where T : new()
        {

            var instance = new T();
            var type = typeof(T);
            foreach (var propInfo in type.GetProperties().Where(d => !d.IsSpecialName))
            {
                var attrs = propInfo.GetCustomAttributes(typeof(FieldAttribute), true);
                if (attrs == null || attrs.Length == 0) continue;
                var attr = attrs[0] as FieldAttribute;
                if (attr == null) continue;
                Set(instance, attr, data, propInfo, type);
            }
            return instance;
        }
        /// <summary>
        /// 根据DATAROW数据设置对象实例的属性
        /// </summary>
        /// <param name="instance">对象实例</param>
        /// <param name="attribute">属性</param>
        /// <param name="data">DATAROW集合</param>
        /// <param name="prop">属性信息</param>
        /// <param name="type">字段类型信息</param>
        static void Set(object instance, FieldAttribute attribute, DataRow data, PropertyInfo prop, Type type)
        {
            DataColumn? column = null;
            if (attribute.IgnoreCase && attribute.Name != null)
            {
                column = data.Table.Columns.Cast<DataColumn>().Where(d => d.ColumnName != null && d.ColumnName.ToLower().Equals(attribute.Name.ToLower())).FirstOrDefault();
            }
            else if (attribute.Name != null)
            {
                column = data.Table.Columns.Cast<DataColumn>().Where(d => d.ColumnName != null && d.ColumnName.Equals(attribute.Name)).FirstOrDefault();
            }
            if (column == null)
            {
                if (attribute.IgnoreCase)
                {
                    column = data.Table.Columns.Cast<DataColumn>().Where(d => d.ColumnName.ToLower().Equals(prop.Name.ToLower())).FirstOrDefault();
                }
                else
                {
                    column = data.Table.Columns.Cast<DataColumn>().Where(d => d.ColumnName.Equals(prop.Name)).FirstOrDefault();
                }

                if (column == null)
                {
                    return;
                }
            }
            var value = data[column];
            var val = value;

            IObjectFormat? formator = null;
            if (attribute.Formator != null)
            {
                formator = Activator.CreateInstance(attribute.Formator) as IObjectFormat;
            }
            if (formator == null)
            {
                formator = new DefaultFormat();
            }

            if (value == null || value == DBNull.Value) return;
            val = formator.Format(column.DataType, val, attribute.Format, prop.PropertyType);
            prop.SetValue(instance, val, new object[0]);
        }
    }
    /// <summary>
    /// 数据格式化契约
    /// </summary>
    public interface IObjectFormat
    {
        /// <summary>
        /// 将类型为inType的值value转换为类型outType，若提供类型format,则outType将根据format执行格式化
        /// </summary>
        /// <param name="inType">入参类型</param>
        /// <param name="value">入参值</param>
        /// <param name="format">格式参数</param>
        /// <param name="outType">出参类型</param>
        /// <returns>转换后的对象</returns>
        object Format(Type inType, object value, string format, Type outType);
    }

    /// <summary>
    /// 默认数据格式化实现
    /// </summary>
    public class DefaultFormat : IObjectFormat
    {
        /// <summary>
        /// 将类型为inType的值value转换为类型outType，若提供类型format,则outType将根据format执行格式化
        /// 1 string
        /// 2 decimal
        /// 3 float
        /// 4 double
        /// 5 int
        /// 6 long 
        /// 7 short
        /// 8 byte
        /// 9 DateTime
        /// 10 Enum枚举
        /// </summary>
        /// <param name="inType">入参类型</param>
        /// <param name="value">入参值</param>
        /// <param name="format">格式参数</param>
        /// <param name="outType">出参类型</param>
        /// <returns>转换后的对象</returns>
        public object Format(Type inType, object value, string format, Type outType)
        {
            if (outType == typeof(string))
            {
                if (inType == typeof(string)) return value;
                if (inType == typeof(decimal)) return string.IsNullOrEmpty(format) ? value.ToString() : ((decimal)value).ToString(format);
                if (inType == typeof(float)) return string.IsNullOrEmpty(format) ? value.ToString() : ((float)value).ToString(format);
                if (inType == typeof(int)) return string.IsNullOrEmpty(format) ? value.ToString() : ((int)value).ToString(format);
                if (inType == typeof(short)) return string.IsNullOrEmpty(format) ? value.ToString() : ((short)value).ToString(format);
                if (inType == typeof(byte)) return string.IsNullOrEmpty(format) ? value.ToString() : ((byte)value).ToString(format);
                if (inType == typeof(long)) return string.IsNullOrEmpty(format) ? value.ToString() : ((long)value).ToString(format);
                if (inType == typeof(double)) return string.IsNullOrEmpty(format) ? value.ToString() : ((double)value).ToString(format);
                if (inType == typeof(bool)) return value.ToString();
                if (inType == typeof(DateTime)) return string.IsNullOrEmpty(format) ? ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss") : ((DateTime)value).ToString(format);
                if (inType == typeof(Guid)) return ((Guid)value).ToString(format);

                if (inType == typeof(decimal?)) return string.IsNullOrEmpty(format) ? value.ToString() : ((decimal?)value).GetValueOrDefault().ToString(format);
                if (inType == typeof(float?)) return string.IsNullOrEmpty(format) ? value.ToString() : ((float?)value).GetValueOrDefault().ToString(format);
                if (inType == typeof(int?)) return string.IsNullOrEmpty(format) ? value.ToString() : ((int?)value).GetValueOrDefault().ToString(format);
                if (inType == typeof(short?)) return string.IsNullOrEmpty(format) ? value.ToString() : ((short?)value).GetValueOrDefault().ToString(format);
                if (inType == typeof(byte?)) return string.IsNullOrEmpty(format) ? value.ToString() : ((byte?)value).GetValueOrDefault().ToString(format);
                if (inType == typeof(long?)) return string.IsNullOrEmpty(format) ? value.ToString() : ((long?)value).GetValueOrDefault().ToString(format);
                if (inType == typeof(double?)) return string.IsNullOrEmpty(format) ? value.ToString() : ((double?)value).GetValueOrDefault().ToString(format);
                if (inType == typeof(bool?)) return ((bool?)value).GetValueOrDefault().ToString();
                if (inType == typeof(DateTime?)) return string.IsNullOrEmpty(format) ? ((DateTime?)value).GetValueOrDefault().ToString("yyyy-MM-dd HH:mm:ss") : ((DateTime?)value).GetValueOrDefault().ToString(format);
                if (inType == typeof(Guid?)) return ((Guid?)value).GetValueOrDefault().ToString(format);

                return value == null ? null : value.ToString();
            }
            else if (outType == typeof(decimal) || outType == typeof(decimal?))
            {
                if (inType == typeof(string)) return decimal.Parse(value.ToString());
                if (inType == typeof(decimal)) return (decimal)value;
                if (inType == typeof(float)) return (decimal)(float)value;
                if (inType == typeof(int)) return (decimal)(int)value;
                if (inType == typeof(short)) return (decimal)(short)value;
                if (inType == typeof(byte)) return (decimal)(byte)value;
                if (inType == typeof(long)) return (decimal)(long)value;
                if (inType == typeof(double)) return (decimal)(double)value;
                if (inType == typeof(bool)) return ((bool)value) ? 1M : 0M;
                if (inType == typeof(DateTime)) return (decimal)((DateTime)value).Ticks;

                if (inType == typeof(decimal?)) return ((decimal?)value).GetValueOrDefault();
                if (inType == typeof(float?)) return (decimal)((float?)value).GetValueOrDefault();
                if (inType == typeof(int?)) return (decimal)((int?)value).GetValueOrDefault();
                if (inType == typeof(short?)) return (decimal)((short?)value).GetValueOrDefault();
                if (inType == typeof(byte?)) return (decimal)((byte?)value).GetValueOrDefault();
                if (inType == typeof(long?)) return (decimal)((long?)value).GetValueOrDefault();
                if (inType == typeof(double?)) return (decimal)((double?)value).GetValueOrDefault();
                if (inType == typeof(bool?)) return ((bool?)value).GetValueOrDefault() ? 1M : 0M;
                if (inType == typeof(DateTime?)) return (decimal)((DateTime?)value).GetValueOrDefault().Ticks;

                return value == null ? 0M : (decimal)value;
            }
            else if (outType == typeof(float) || outType == typeof(float?))
            {
                if (inType == typeof(string)) return float.Parse(value.ToString());
                if (inType == typeof(decimal)) return (float)(decimal)value;
                if (inType == typeof(float)) return (float)value;
                if (inType == typeof(int)) return (float)(int)value;
                if (inType == typeof(short)) return (float)(short)value;
                if (inType == typeof(byte)) return (float)(byte)value;
                if (inType == typeof(long)) return (float)(long)value;
                if (inType == typeof(double)) return (float)(double)value;
                if (inType == typeof(bool)) return ((bool)value) ? 1f : 0f;
                if (inType == typeof(DateTime)) return (float)((DateTime)value).Ticks;

                if (inType == typeof(decimal?)) return (float)((decimal?)value).GetValueOrDefault();
                if (inType == typeof(float?)) return ((float?)value).GetValueOrDefault();
                if (inType == typeof(int?)) return (float)((int?)value).GetValueOrDefault();
                if (inType == typeof(short?)) return (float)((short?)value).GetValueOrDefault();
                if (inType == typeof(byte?)) return (float)((byte?)value).GetValueOrDefault();
                if (inType == typeof(long?)) return (float)((long?)value).GetValueOrDefault();
                if (inType == typeof(double?)) return (float)((double?)value).GetValueOrDefault();
                if (inType == typeof(bool?)) return ((bool?)value).GetValueOrDefault() ? 1f : 0f;
                if (inType == typeof(DateTime?)) return (float)((DateTime?)value).GetValueOrDefault().Ticks;

                return value == null ? 0.0f : (float)value;
            }
            else if (outType == typeof(double) || outType == typeof(double?))
            {
                if (inType == typeof(string)) return double.Parse(value.ToString());
                if (inType == typeof(decimal)) return (double)(decimal)value;
                if (inType == typeof(float)) return (double)(float)value;
                if (inType == typeof(int)) return (double)(int)value;
                if (inType == typeof(short)) return (double)(short)value;
                if (inType == typeof(byte)) return (double)(byte)value;
                if (inType == typeof(long)) return (double)(long)value;
                if (inType == typeof(double)) return (double)(double)value;
                if (inType == typeof(bool)) return ((bool)value) ? 1.0 : 0.0d;
                if (inType == typeof(DateTime)) return (double)((DateTime)value).Ticks;

                if (inType == typeof(decimal?)) return (double)((decimal?)value).GetValueOrDefault();
                if (inType == typeof(float?)) return (double)((float?)value).GetValueOrDefault();
                if (inType == typeof(int?)) return (double)((int?)value).GetValueOrDefault();
                if (inType == typeof(short?)) return (double)((short?)value).GetValueOrDefault();
                if (inType == typeof(byte?)) return (double)((byte?)value).GetValueOrDefault();
                if (inType == typeof(long?)) return (double)((long?)value).GetValueOrDefault();
                if (inType == typeof(double?)) return ((double?)value).GetValueOrDefault();
                if (inType == typeof(bool?)) return ((bool?)value).GetValueOrDefault() ? 1.0 : 0.0d;
                if (inType == typeof(DateTime?)) return (double)((DateTime?)value).GetValueOrDefault().Ticks;

                return value == null ? 0.0d : (double)value;
            }
            else if (outType == typeof(int) || outType == typeof(int?))
            {
                if (inType == typeof(string)) return (int)double.Parse(value.ToString());
                if (inType == typeof(decimal)) return (int)(decimal)value;
                if (inType == typeof(float)) return (int)(float)value;
                if (inType == typeof(int)) return (int)value;
                if (inType == typeof(short)) return (int)(short)value;
                if (inType == typeof(byte)) return (int)(byte)value;
                if (inType == typeof(long)) return (int)(long)value;
                if (inType == typeof(double)) return (int)(double)value;
                if (inType == typeof(bool)) return ((bool)value) ? 1 : 0;
                if (inType == typeof(DateTime)) return (int)((DateTime)value).Ticks;

                if (inType == typeof(decimal?)) return (int)((decimal?)value).GetValueOrDefault();
                if (inType == typeof(float?)) return (int)((float?)value).GetValueOrDefault();
                if (inType == typeof(int?)) return ((int?)value).GetValueOrDefault();
                if (inType == typeof(short?)) return (double)((short?)value).GetValueOrDefault();
                if (inType == typeof(byte?)) return (int)((byte?)value).GetValueOrDefault();
                if (inType == typeof(long?)) return (int)((long?)value).GetValueOrDefault();
                if (inType == typeof(double?)) return (int)((double?)value).GetValueOrDefault();
                if (inType == typeof(bool?)) return ((bool?)value).GetValueOrDefault() ? 1.0 : 0.0f;
                if (inType == typeof(DateTime?)) return (int)((DateTime?)value).GetValueOrDefault().Ticks;

                return value == null ? 0 : (int)value;
            }
            else if (outType == typeof(long) || outType == typeof(long?))
            {
                if (inType == typeof(string)) return (long)double.Parse(value.ToString());
                if (inType == typeof(decimal)) return (int)(decimal)value;
                if (inType == typeof(float)) return (long)(float)value;
                if (inType == typeof(int)) return (long)(int)value;
                if (inType == typeof(short)) return (long)(short)value;
                if (inType == typeof(byte)) return (long)(byte)value;
                if (inType == typeof(long)) return (long)value;
                if (inType == typeof(double)) return (long)(double)value;
                if (inType == typeof(bool)) return ((bool)value) ? 1L : 0L;
                if (inType == typeof(DateTime)) return ((DateTime)value).Ticks;

                if (inType == typeof(decimal?)) return (long)((decimal?)value).GetValueOrDefault();
                if (inType == typeof(float?)) return (long)((float?)value).GetValueOrDefault();
                if (inType == typeof(int?)) return (long)((int?)value).GetValueOrDefault();
                if (inType == typeof(short?)) return (long)((short?)value).GetValueOrDefault();
                if (inType == typeof(byte?)) return (long)((byte?)value).GetValueOrDefault();
                if (inType == typeof(long?)) return ((long?)value).GetValueOrDefault();
                if (inType == typeof(double?)) return (long)((double?)value).GetValueOrDefault();
                if (inType == typeof(bool?)) return ((bool?)value).GetValueOrDefault() ? 1L : 0L;
                if (inType == typeof(DateTime?)) return ((DateTime?)value).GetValueOrDefault().Ticks;

                return value == null ? 0L : (long)value;
            }
            else if (outType == typeof(short) || outType == typeof(short?))
            {
                if (inType == typeof(string)) return (short)double.Parse(value.ToString());
                if (inType == typeof(decimal)) return (short)(decimal)value;
                if (inType == typeof(float)) return (short)(float)value;
                if (inType == typeof(int)) return (short)(int)value;
                if (inType == typeof(short)) return (short)value;
                if (inType == typeof(byte)) return (short)(byte)value;
                if (inType == typeof(long)) return (short)(long)value;
                if (inType == typeof(double)) return (short)(double)value;
                if (inType == typeof(bool)) return ((bool)value) ? 1 : 0;
                if (inType == typeof(DateTime)) return (short)((DateTime)value).Ticks;

                if (inType == typeof(decimal?)) return (short)((decimal?)value).GetValueOrDefault();
                if (inType == typeof(float?)) return (short)((float?)value).GetValueOrDefault();
                if (inType == typeof(int?)) return (short)((int?)value).GetValueOrDefault();
                if (inType == typeof(short?)) return ((short?)value).GetValueOrDefault();
                if (inType == typeof(byte?)) return (short)((byte?)value).GetValueOrDefault();
                if (inType == typeof(long?)) return (short)((long?)value).GetValueOrDefault();
                if (inType == typeof(double?)) return (short)((double?)value).GetValueOrDefault();
                if (inType == typeof(bool?)) return ((bool?)value).GetValueOrDefault() ? 1 : 0;
                if (inType == typeof(DateTime?)) return (short)((DateTime?)value).GetValueOrDefault().Ticks;

                return value == null ? 0 : (short)value;
            }
            else if (outType == typeof(byte) || outType == typeof(byte?))
            {
                if (inType == typeof(string)) return (byte)double.Parse(value.ToString());
                if (inType == typeof(decimal)) return (byte)(decimal)value;
                if (inType == typeof(float)) return (byte)(float)value;
                if (inType == typeof(int)) return (byte)(int)value;
                if (inType == typeof(short)) return (byte)(short)value;
                if (inType == typeof(byte)) return (byte)value;
                if (inType == typeof(long)) return (byte)(long)value;
                if (inType == typeof(double)) return (byte)(double)value;
                if (inType == typeof(bool)) return ((bool)value) ? 1 : 0;
                if (inType == typeof(DateTime)) return (byte)((DateTime)value).Ticks;

                if (inType == typeof(decimal?)) return (byte)((decimal?)value).GetValueOrDefault();
                if (inType == typeof(float?)) return (byte)((float?)value).GetValueOrDefault();
                if (inType == typeof(int?)) return (byte)((int?)value).GetValueOrDefault();
                if (inType == typeof(short?)) return (byte)((short?)value).GetValueOrDefault();
                if (inType == typeof(byte?)) return ((byte?)value).GetValueOrDefault();
                if (inType == typeof(long?)) return (byte)((long?)value).GetValueOrDefault();
                if (inType == typeof(double?)) return (byte)((double?)value).GetValueOrDefault();
                if (inType == typeof(bool?)) return ((bool?)value).GetValueOrDefault() ? 1 : 0;
                if (inType == typeof(DateTime?)) return (byte)((DateTime?)value).GetValueOrDefault().Ticks;

                return value == null ? 0 : (byte)value;
            }
            else if (outType == typeof(DateTime) || outType == typeof(DateTime?))
            {
                if (inType == typeof(string)) return ((string)value).IsDigit() ? new DateTime((long)double.Parse((string)value)) : DateTime.Parse((string)value);
                if (inType == typeof(decimal)) return new DateTime((long)(decimal)value);
                if (inType == typeof(float)) return new DateTime((long)(float)value);
                if (inType == typeof(int)) return new DateTime((long)(int)value);
                if (inType == typeof(short)) return new DateTime((long)(short)value);
                if (inType == typeof(byte)) return new DateTime((long)(byte)value);
                if (inType == typeof(long)) return new DateTime((long)(long)value);
                if (inType == typeof(double)) return new DateTime((long)(double)value);
                if (inType == typeof(bool)) return ((bool)value) ? DateTime.MaxValue : DateTime.MinValue;
                if (inType == typeof(DateTime)) return (DateTime)value;

                if (inType == typeof(decimal?)) return new DateTime((long)(decimal?)value);
                if (inType == typeof(float?)) return new DateTime((long)((float?)value).GetValueOrDefault());
                if (inType == typeof(int?)) return new DateTime((long)((int?)value).GetValueOrDefault());
                if (inType == typeof(short?)) return new DateTime((long)((short?)value).GetValueOrDefault());
                if (inType == typeof(byte?)) return new DateTime((long)((byte?)value).GetValueOrDefault());
                if (inType == typeof(long?)) return new DateTime((long)((long?)value).GetValueOrDefault());
                if (inType == typeof(double?)) return new DateTime((long)((double?)value).GetValueOrDefault());
                if (inType == typeof(bool?)) return ((bool?)value).GetValueOrDefault() ? DateTime.MaxValue : DateTime.MinValue;
                if (inType == typeof(DateTime?)) return ((DateTime?)value).GetValueOrDefault();

                return value == null ? DateTime.MinValue : (DateTime)value;
            }
            else if (outType == typeof(bool) || outType == typeof(bool?))
            {
                if (inType == typeof(string)) return ((string)value).IsDigit() ? ((long)double.Parse((string)value)) != 0 : bool.Parse(value.ToString());
                if (inType == typeof(decimal)) return ((decimal)value) != 0;
                if (inType == typeof(float)) return ((float)value) != 0;
                if (inType == typeof(int)) return ((int)value) != 0;
                if (inType == typeof(short)) return ((short)value) != 0;
                if (inType == typeof(byte)) return ((byte)value) != 0;
                if (inType == typeof(long)) return ((long)value) != 0;
                if (inType == typeof(double)) return ((double)value) != 0;
                if (inType == typeof(bool)) return (bool)value;
                if (inType == typeof(DateTime)) return ((DateTime)value) != DateTime.MinValue;

                if (inType == typeof(decimal?)) return ((decimal?)value).GetValueOrDefault() != 0;
                if (inType == typeof(float?)) return ((float?)value).GetValueOrDefault() != 0;
                if (inType == typeof(int?)) return ((int?)value).GetValueOrDefault() != 0;
                if (inType == typeof(short?)) return ((short?)value).GetValueOrDefault() != 0;
                if (inType == typeof(byte?)) return ((byte?)value).GetValueOrDefault() != 0;
                if (inType == typeof(long?)) return ((long?)value).GetValueOrDefault() != 0;
                if (inType == typeof(double?)) return ((double?)value).GetValueOrDefault() != 0;
                if (inType == typeof(bool?)) return ((bool?)value).GetValueOrDefault();
                if (inType == typeof(DateTime?)) return ((DateTime?)value).GetValueOrDefault() != DateTime.MinValue;

                return value == null ? false : (bool)value;
            }
            else if (outType.IsEnum || outType.IsAssignableFrom(typeof(Enum)))
            {
                if (inType == typeof(string)) return Enum.Parse(outType, value.ToString());
                if (inType == typeof(decimal)) return Enum.Parse(outType, ((int)(decimal)value).ToString());
                if (inType == typeof(float)) return Enum.Parse(outType, ((int)(float)value).ToString());
                if (inType == typeof(int)) return Enum.Parse(outType, ((int)(int)value).ToString());
                if (inType == typeof(short)) return Enum.Parse(outType, ((int)(short)value).ToString());
                if (inType == typeof(byte)) return Enum.Parse(outType, ((int)(byte)value).ToString());
                if (inType == typeof(long)) return Enum.Parse(outType, ((int)(long)value).ToString());
                if (inType == typeof(double)) return Enum.Parse(outType, ((int)(double)value).ToString());
                if (inType == typeof(bool)) return Enum.Parse(outType, ((bool)value).ToString());
                if (inType == typeof(DateTime)) return Enum.Parse(outType, ((DateTime)value).Ticks.ToString());

                if (inType == typeof(decimal?)) return Enum.Parse(outType, ((int)((decimal?)value).GetValueOrDefault()).ToString());
                if (inType == typeof(float?)) return Enum.Parse(outType, ((int)((float?)value).GetValueOrDefault()).ToString());
                if (inType == typeof(int?)) return Enum.Parse(outType, (((int?)value).GetValueOrDefault()).ToString());
                if (inType == typeof(short?)) return Enum.Parse(outType, ((int)((short?)value).GetValueOrDefault()).ToString());
                if (inType == typeof(byte?)) return Enum.Parse(outType, ((int)((byte?)value).GetValueOrDefault()).ToString());
                if (inType == typeof(long?)) return Enum.Parse(outType, ((int)((long?)value).GetValueOrDefault()).ToString());
                if (inType == typeof(double?)) return Enum.Parse(outType, ((int)((double?)value).GetValueOrDefault()).ToString());
                if (inType == typeof(bool?)) return Enum.Parse(outType, ((bool?)value).GetValueOrDefault().ToString());
                if (inType == typeof(DateTime?)) return Enum.Parse(outType, ((DateTime)value).Ticks.ToString());
            }

            return value;
        }
    }

    /// <summary>
    /// 字段属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldAttribute : Attribute
    {
        string category;
        /// <summary>
        /// 类别
        /// </summary>
        public string Category
        {
            get { return category; }
            set { category = value; }
        }


        string name;
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        string format;

        /// <summary>
        /// 字段格式
        /// </summary>
        public string Format
        {
            get { return format; }
            set { format = value; }
        }
        Type formator;

        /// <summary>
        /// 自定义格式化类型,需要实现IObjectFormat，默认为DefaultFormat
        /// </summary>
        public Type Formator
        {
            get { return formator; }
            set { formator = value; }
        }
        bool ignoreCase = true;

        /// <summary>
        /// 是否不区分大小写
        /// </summary>
        public bool IgnoreCase
        {
            get { return ignoreCase; }
            set { ignoreCase = value; }
        }

        public FieldAttribute() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="name"></param>
        /// <param name="category"></param>
        /// <param name="format"></param>
        /// <param name="formator"></param>
        /// <param name="ignoreCase"></param>
        public FieldAttribute(string name, string category = null, string format = null, Type formator = null, bool ignoreCase = true)
        {
            this.name = name;
            this.category = category;
            this.format = format;
            this.formator = formator;
            this.ignoreCase = ignoreCase;
        }
    }
}