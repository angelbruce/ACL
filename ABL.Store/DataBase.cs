using ABL.Data;
using ABL.Object;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace ABL.Store
{
    public class DataBase
    {
        protected DbConnection connection;
        protected DbTransaction transaction;
        protected DbDriver creator;

        public IDbConnection Connection
        {
            get { return connection; }
        }

        public DbTransaction Transaction
        {
            get { return transaction; }
        }

        public DbDriver Creator
        {
            get { return creator; }
        }

        public DataBase(DbDriver creator)
        {
            this.creator = creator;
            this.connection = this.creator.CreateConnection();
            this.connection.ConnectionString = creator.ConnectionString;
        }

        public DataBase(string connectionString, DbDriver creator)
        {
            this.creator = creator;
            this.connection = creator.CreateConnection();
            this.connection.ConnectionString = connectionString;
        }

        public DataBase(DbConnection connection, DbDriver creator)
        {
            this.creator = creator;
            this.connection = connection;
        }

        public DataBase(DataBase dbUtility)
        {
            this.connection = dbUtility.connection;
            this.transaction = dbUtility.transaction;
            this.creator = dbUtility.creator;
        }

        public void Open()
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
        }

        public void Close()
        {
            if (connection.State != ConnectionState.Closed)
                connection.Close();
        }

        public virtual void BeginTransaction()
        {
            if (transaction == null)
                transaction = connection.BeginTransaction();
        }

        public virtual void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (transaction == null)
                transaction = connection.BeginTransaction(isolationLevel);
        }

        public virtual void Commit()
        {
            if (transaction == null)
                return;
            transaction.Commit();
            transaction = null;
        }

        public virtual void Rollback()
        {
            if (transaction == null)
                return;
            transaction.Rollback();
            transaction = null;
        }

        public int ExecuteNonQuery(string commandText, CommandType commandType = CommandType.Text, DbParameter[] parameters = null)
        {
            var command = PrepareCommand(commandText, commandType, parameters);
            return command.ExecuteNonQuery();
        }

        public DbDataReader ExecuteReader(string commandText, CommandType commandType = CommandType.Text, DbParameter[] parameters = null)
        {
            var command = PrepareCommand(commandText, commandType, parameters);
            return command.ExecuteReader();
        }

        public DbDataReader ExecuteReader(string commandText, CommandBehavior commandBehavior, CommandType commandType = CommandType.Text, DbParameter[] parameters = null)
        {
            var command = PrepareCommand(commandText, commandType, parameters);
            return command.ExecuteReader(commandBehavior);
        }

        public object ExecuteSclar(string commandText, CommandType commandType = CommandType.Text, DbParameter[] parameters = null)
        {
            var command = PrepareCommand(commandText, commandType, parameters);
            return command.ExecuteScalar();
        }

        public DataSet QueryDataSet(string commandText, CommandType commandType = CommandType.Text, DbParameter[] parameters = null, string srcTable = null, int startRecord = -1, int maxRecords = -1)
        {
            var adapter = creator.CreateAdapter();
            adapter.SelectCommand = PrepareCommand(commandText, commandType, parameters);
            var ds = new DataSet();
            if (string.IsNullOrEmpty(srcTable))
            {
                adapter.Fill(ds);
            }
            else if (startRecord == -1)
            {
                adapter.Fill(ds, startRecord, maxRecords, srcTable);
            }
            else
            {
                adapter.Fill(ds, srcTable);
            }
            return ds;
        }

        public DataTable QueryDataTable(string commandText, CommandType commandType = CommandType.Text, DbParameter[] parameters = null, string tableName = "table0")
        {
            var adapter = creator.CreateAdapter();
            adapter.SelectCommand = PrepareCommand(commandText, commandType, parameters);
            DataTable dt = new DataTable(tableName);
            adapter.Fill(dt);
            return dt;
        }

        public List<T> Fill<T>(string commandText = null, bool safeRead = false, CommandType commandType = CommandType.Text, DbParameter[] parameters = null) where T : AbstractData, new()
        {
            DbDataReader reader = null;
            var map = MapCollector.Get(typeof(T));
            var reverseMap = new Dictionary<string, string>();
            map.Fields.ForEach(d => reverseMap.Add(d.Name, d.PropertyName));
            reader = string.IsNullOrEmpty(commandText)
                ? this.ExecuteReader(map.ToSelect())
                : this.ExecuteReader(commandText, commandType, parameters);

            var list = new List<T>();
            try
            {
                var readColumns = map.Fields.Select(d => d.Name).ToList();
                if (safeRead)
                {
                    var columns = Enumerable.Range(0, reader.FieldCount).Select(i => reader.GetDataTypeName(i)).ToList();
                    readColumns = readColumns.Intersect(columns).ToList();
                }
                while (reader.Read())
                {
                    var t = new T();
                    readColumns.ForEach(column => t.Set(reverseMap[column], reader[column]));
                    list.Add(t);
                }
            }
            finally
            {
                reader.Close();
            }
            return list;
        }

        public int Save<T>(T data, bool safeWrite = false, string selectCommandText = null, CommandType commandType = CommandType.Text, DbParameter[] parameters = null) where T : AbstractData, new()
        {
            if (data == null) return 0;
            return Save(new List<T> { data }, safeWrite, selectCommandText, commandType, parameters);
        }

        protected virtual void OnCreateInsertCommand(DbDataAdapter adapter, DbCommand command, MapInfo map, List<PKMemo> pks)
        {
            command.UpdatedRowSource = UpdateRowSource.FirstReturnedRecord;
        }

        public class PKMemo
        {
            public object OldPk { get; set; }
            public object NewPk { get; set; }
        }

        public int Save<T>(List<T> datas, bool safeWrite = false, string selectCommandText = null, CommandType commandType = CommandType.Text, DbParameter[] parameters = null) where T : AbstractData, new()
        {
            if (datas == null) return 0;
            var pks = new List<PKMemo>();
            try
            {
                BeginTransaction();
                var table = new DataTable();
                var map = MapCollector.Get(typeof(T));
                var columns = map.Fields.Select(d => d.Name).ToList();
                var adapter = creator.CreateAdapter();
                adapter.SelectCommand = PrepareCommand(string.IsNullOrEmpty(selectCommandText) ? map.ToSelect() : selectCommandText, commandType, parameters);
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapter.FillSchema(table, SchemaType.Mapped);
                var builder = creator.CreateCommandBuilder();
                builder.DataAdapter = adapter;
                builder.SetAllValues = true;
                builder.ConflictOption = ConflictOption.OverwriteChanges;
                adapter.UpdateCommand = builder.GetUpdateCommand();
                adapter.InsertCommand = builder.GetInsertCommand();
                if (map.Pk != null) table.PrimaryKey = new DataColumn[] { table.Columns[map.Pk.Name] };
                if (adapter.InsertCommand != null) OnCreateInsertCommand(adapter, adapter.InsertCommand, map, pks);

                adapter.DeleteCommand = builder.GetDeleteCommand();
                adapter.ContinueUpdateOnError = false;
                var reverseMap = new Dictionary<string, string>();
                map.Fields.ForEach(d => reverseMap.Add(d.Name, d.PropertyName));
                if (safeWrite)
                {
                    var schemas = table.Columns.Cast<DataColumn>().Select(d => d.ColumnName).ToList();
                    columns = columns.Intersect(schemas).ToList();
                }
                var time = DateTime.Now;
                table.BeginLoadData();
                foreach (var data in datas)
                {
                    if (data.State == ABL.Object.EnumEntityState.None
                        || data.State == ABL.Object.EnumEntityState.Detached
                        || data.State == ABL.Object.EnumEntityState.Attached) continue;
                    var row = table.NewRow();
                    foreach (var column in columns)
                    {
                        var value = data.Inner(reverseMap[column]);
                        row[column] = value ?? DBNull.Value;
                    }
                    table.Rows.Add(row);
                    row.AcceptChanges();
                    switch (data.State)
                    {
                        case ABL.Object.EnumEntityState.Added:
                            row.SetAdded();
                            break;
                        case ABL.Object.EnumEntityState.Modified:
                            row.SetModified();
                            break;
                        case ABL.Object.EnumEntityState.Deleted:
                            row.Delete();
                            break;
                    }
                }
                table.EndLoadData();
                var end = DateTime.Now - time;
                System.Diagnostics.Trace.WriteLine("filll in table:" + end.TotalSeconds.ToString());

                try
                {
                    time = DateTime.Now;
                    int count = adapter.Update(table);
                    Commit();

                    SetNewPks(pks, datas, map);
                    return count;
                }
                finally
                {
                    OnReleaseEvent(adapter, map);
                    end = DateTime.Now - time;
                    System.Diagnostics.Trace.WriteLine("updated:" + end.TotalSeconds.ToString());
                }

            }
            catch
            {
                Rollback();
                throw;
            }
        }

        protected virtual void SetNewPks<T>(List<PKMemo> pKMemos, List<T> datas, MapInfo map)
            where T : AbstractData
        {
        }

        protected virtual void OnReleaseEvent(DbDataAdapter adapter, MapInfo map) { }
        protected DbCommand PrepareCommand(string commandText, CommandType commandType = CommandType.Text, DbParameter[] parameters = null)
        {
            var command = creator.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = commandText;
            command.CommandType = commandType;
            command.Connection = connection;
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            if (parameters != null && parameters.Length > 0)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
            return command;
        }
    }
}
