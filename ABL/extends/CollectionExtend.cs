using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace ABL
{
    public static class CollectionExtend
    {
        /// <summary>
        /// 为每个元素执行action并返回执行后的元素集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) where T : class
        {
            if (action == null || enumerable == null) return enumerable;
            var iter = enumerable.GetEnumerator();
            while (iter.MoveNext()) action(iter.Current);
            return enumerable;
        }


        public static IDictionary<TKey, TValue> ForEach<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Action<TKey, TValue> action)
        {
            if (action == null || dictionary == null) return dictionary;
            var iter = dictionary.GetEnumerator();
            foreach (var pair in dictionary) action(pair.Key, pair.Value);
            return dictionary;
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> enumerable) where T : class
        {
            if (enumerable == null) return null;
            var type = typeof(T);
            var table = new DataTable();
            table.TableName = type.Name;
            var props = type.GetProperties().Where(p => !p.IsSpecialName).ToList();
            foreach (var prop in props)
            {
                var propType = prop.PropertyType;
                if (propType.IsGenericType) table.Columns.Add(prop.Name, propType.GetGenericArguments()[0]);
                else table.Columns.Add(prop.Name, propType);
            }
            var enumerator = enumerable.GetEnumerator();
            table.BeginLoadData();
            while (enumerator.MoveNext())
            {
                var data = enumerator.Current;
                if (data == null) continue;
                var row = table.NewRow();
                table.Rows.Add(row);
                foreach (var prop in props)
                {
                    var value = prop.GetValue(data, new object[0]);
                    row[prop.Name] = value == null ? DBNull.Value : value;
                }
            }
            table.EndLoadData();
            return table;
        }

        static void GenerateTableSchema(DataSet ds, Dictionary<string, DataTable> paths, DataTable parent, System.Reflection.PropertyInfo property, Type type)
        {
            var table = new DataTable();
            table.TableName = parent != null ? string.Format("{0}_{1}", parent.TableName, property.Name) : type.Name;
            if (parent != null)
            {
                parent.Columns.Add(table.TableName);
                table.Columns.Add(table.TableName);
            }
            paths.Add(table.TableName, table);
            var props = type.GetProperties().Where(p => !p.IsSpecialName).ToList();
            var enumType = typeof(IEnumerable);
            foreach (var prop in props)
            {
                table.Columns.Add(prop.Name, prop.PropertyType);
                var pt = prop.PropertyType;
                if (pt.IsPrimitive) continue;
                if (!pt.IsClass) continue;
                if (!typeof(IEnumerable).IsAssignableFrom(pt)) continue;
                var ntype = pt.IsGenericType ? pt.GetGenericArguments()[0] : pt.GetElementType();
                if (ntype == null) continue;
                table.Columns.Remove(prop.Name);
                GenerateTableSchema(ds, paths, table, prop, ntype);
            }
        }

        static void GenerateTableData(Dictionary<string, DataTable> paths, DataRow parent, System.Reflection.PropertyInfo property, Type type, IEnumerable datas, Guid guid)
        {
            if (datas == null) return;
            var path = parent == null ? type.Name : string.Format("{0}_{1}", parent.Table.TableName, property.Name);
            var table = paths[path];
            var props = type.GetProperties().Where(p => !p.IsSpecialName).ToList();
            var enumerator = datas.GetEnumerator();
            bool filled = false;
            while (enumerator.MoveNext())
            {
                var row = table.NewRow();
                table.Rows.Add(row);
                var data = enumerator.Current;
                if (parent != null)
                {
                    row[table.TableName] = guid;
                    if (!filled)
                    {
                        parent[table.TableName] = guid;
                        filled = true;
                    }
                }
                foreach (var prop in props)
                {
                    if (table.Columns.Contains(prop.Name))
                    {
                        row[prop.Name] = prop.GetValue(data, new object[0]);
                        continue;
                    }
                    var relation = Guid.NewGuid();
                    var value = prop.GetValue(data, new object[0]) as IEnumerable;
                    var ntype = prop.PropertyType.IsGenericType ? prop.PropertyType.GetGenericArguments()[0] : prop.PropertyType.GetElementType();
                    GenerateTableData(paths, row, prop, ntype, value, relation);
                }
            }
        }

        public static DataSet ToDataSet<T>(this IEnumerable<T> enumerable) where T : class
        {
            if (enumerable == null) return null;
            var ds = new DataSet();
            var paths = new Dictionary<string, DataTable>();
            var type = typeof(T);
            GenerateTableSchema(ds, paths, null, null, type);
            foreach (var pair in paths) ds.Tables.Add(pair.Value);
            GenerateTableData(paths, null, null, type, enumerable, Guid.Empty);
            return ds;
        }
    }
}
