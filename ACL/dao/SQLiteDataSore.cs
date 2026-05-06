using ABL;
using ABL.Data;
using ABL.Object;
using ABL.Store;
using ACL.business.agent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Text;

namespace ACL.dao
{
    public class SQLiteDataSore : DataBase
    {
        public SQLiteDataSore() : base(DbDriver.Create())
        {
        }

        public override void BeginTransaction()
        {
        }

        public override void Rollback()
        {
        }

        public override void Commit()
        {
        }


        protected override void OnCreateInsertCommand(DbDataAdapter adapter, DbCommand command, MapInfo map, List<PKMemo> pks)
        {
            if (map != null && map.Pk != null && adapter is SQLiteDataAdapter sqliteAdapter)
            {
                sqliteAdapter.RowUpdated += (sender, e) =>
                {
                    if (e.StatementType == StatementType.Insert && e.Status == UpdateStatus.Continue)
                    {
                        // 获取当前连接
                        var connection = e.Command.Connection;

                        // 创建新命令获取最后插入的 ID
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "SELECT last_insert_rowid();";

                            // 确保连接是打开的
                            if (connection.State != ConnectionState.Open)
                                connection.Open();

                            object result = cmd.ExecuteScalar();
                            if (result != null && result != DBNull.Value)
                            {
                                // 将 ID 赋值给 DataRow 的主键列
                                pks.Add(new PKMemo { NewPk = result, OldPk = e.Row[map.Pk.Name] });
                                e.Row[map.Pk.Name] = Convert.ToInt64(result);
                            }
                        }
                    }
                };

                command.CommandText += $";select last_insert_rowid() as {map.Pk.Name};";
            }

            base.OnCreateInsertCommand(adapter, command, map, pks);
        }
        protected override void OnReleaseEvent(DbDataAdapter adapter, MapInfo map)
        {
            if (adapter is SQLiteDataAdapter sqliteAdapter)
            {
                sqliteAdapter.RowUpdated -= null;
            }
        }

        protected override void SetNewPks<T>(List<PKMemo> pks, List<T> datas, MapInfo map)
        {
            if (map.Pk == null) return;
            if (datas == null || datas.Count == 0) return;
            if (pks == null || pks.Count == 0) return;
            var propName = map.Pk.PropertyName;
            var prop = typeof(T).GetProperty(propName);


            var dict = pks.ToDictionary(x => (long)x.OldPk, x => (long)x.NewPk);

            foreach (var data in datas)
            {
                var val = (long)prop.GetValue(data);
                if (dict.ContainsKey(val))
                {
                    prop.SetValue(data, dict[val]);
                }

            }
            foreach (var pk in pks)
            {
                var src = pk.OldPk;
                var dst = pk.NewPk;
                datas.Set(propName, pk.NewPk);
            }

        }

    }


}
