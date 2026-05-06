using ABL.Data;
using ABL.Object;
using ABL.Store;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Text;

namespace ACL.dao
{
    public class SQLiteDbDriver : DbDriver
    {
        public override string CounterStmt(string sql)
        {
            return $"SELECT COUNT(*) FROM ({sql}) AS subquery";
        }

        public override DbDataAdapter CreateAdapter()
        {
            return new SQLiteDataAdapter();
        }

        public override DbCommand CreateCommand()
        {
            return new SQLiteCommand();
        }

        public override DbConnection CreateConnection()
        {
            return new SQLiteConnection();
        }

        public override DbParameter CreateDataParameter()
        {
            return new SQLiteParameter();
        }

        public override DbCommandBuilder CreateCommandBuilder()
        {
            return new SQLiteCommandBuilder();
        }

        public override string NowStmt()
        {
            return "DATETIME('now')";
        }

        public override string PagerStmt(string sql, int index, int size)
        {
            int offset = (index - 1) * size;
            return $"{sql} LIMIT {size} OFFSET {offset}";
        }

        public override string GetDataType(Type type)
        {
            var dataType = "TEXT‌";
            if (type == typeof(int)
                || type == typeof(long)
                || type == typeof(byte)
                || type == typeof(uint)
                || type == typeof(short)
                || type == typeof(ushort)

                || type == typeof(Int32)
                || type == typeof(Int64)
                || type == typeof(Byte)
                || type == typeof(UInt32)
                || type == typeof(Int16)
                || type == typeof(UInt16)

                ) dataType = "INTEGER";
            else if (type == typeof(float)
                || type == typeof(double)
                || type == typeof(decimal)

                || type == typeof(Single)
                || type == typeof(Double)
                || type == typeof(Decimal)
                ) dataType = "REAL‌";
            else if (type == typeof(string)
                || type == typeof(String)
                || type == typeof(StringBuilder)
                || type == typeof(DateTime)
                ) dataType = "TEXT";
            else if (type == typeof(byte[])
                || type == typeof(Byte[])
                ) dataType = "BLOB‌";

            return dataType;
        }


        /// <summary>
        /// 创建schema DDL
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override string CreateSchemaDDL(Schema schema)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// table DDL
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override List<string> CreateTableDDL(Table table)
        {
            var sqls = new List<string>();
            switch (table.State)
            {
                case ABL.Object.EnumEntityState.Added:
                    {
                        var sql = new StringBuilder();
                        sql.Append($"create table {table.Name} (");
                        foreach (var col in table.Columns)
                        {
                            sql.Append($" {col.Name} {col.DataType}");
                            if (col.IsPK) sql.Append($" PRIMARY KEY");
                            if (col.KeyGenerateStrategy == PrimaryKeyGenerateStrategy.AutoIncement)
                            {
                                sql.Append($" AUTOINCREMENT");
                            }
                            sql.Append(",");
                        }

                        if (table.Columns.Count > 0) sql.Length--;
                        sql.Append(");");

                        sqls.Add(sql.ToString());

                        break;
                    }

                case ABL.Object.EnumEntityState.Modified:
                    {
                        foreach (var col in table.Columns)
                        {
                            switch (col.State)
                            {
                                case ABL.Object.EnumEntityState.Added:
                                    {
                                        var sql = new StringBuilder();
                                        sql.Append($" alter table {table.Name} add {col.Name} {col.DataType}");
                                        if (col.IsPK) sql.Append($" PRIMARY KEY");
                                        if (col.KeyGenerateStrategy == PrimaryKeyGenerateStrategy.AutoIncement)
                                        {
                                            sql.Append($" AUTOINCREMENT");
                                        }
                                        sql.Append(";");
                                        sqls.Add(sql.ToString());

                                        break;
                                    }
                            }
                        }

                        break;
                    }
            }

            return sqls;
        }
        /// <summary>
        /// 获取schemas
        /// </summary>
        /// <returns></returns>
        public override List<Schema> FetchSchemas(object data)
        {
            var db = data as DataBase;
            if (db == null) return new List<Schema>();

            var schema = new Schema();

            var sql = "SELECT name as table_name, '' as table_comment FROM sqlite_master WHERE type = 'table'";
            var tables = db.Fill<Table>(sql);

            if (tables != null && tables.Count > 0)
            {
                foreach (var table in tables)
                {
                    var csql = $"PRAGMA table_info({table.Name})";
                    var cols = db.Fill<SQLiteColumn>(csql);
                    foreach (var col in cols)
                    {
                        var column = new Column()
                        {
                            Name = col.Name,
                            DataType = col.DataType,
                            IsPK = col.pk == 1,
                            KeyGenerateStrategy = (col.pk == 1 && col.DataType == "INTEGER") ? PrimaryKeyGenerateStrategy.AutoIncement : PrimaryKeyGenerateStrategy.None
                        };
                        table.Columns.Add(column);
                    }

                    schema.Tables.Add(table);
                }
            }

            return new List<Schema>() { schema };
        }


        [Table]
        public class SQLiteColumn : AbstractData
        {
            [Column("name")]
            public string Name { get; set; } = string.Empty;

            [Column("type")]
            public string DataType { get; set; } = string.Empty;

            [Column("pk")]
            public int pk { get; set; } = 0;
        }
    }
}
