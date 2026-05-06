using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ABL.Data
{
    /// <summary>
    /// 数据库操作
    /// </summary>
    public class Database : IDisposable
    {
        DbConnection connection = null;
        DbDriver driver = null;

        /// <summary>
        /// 数据库连接串
        /// </summary>
        public string ConnectionString
        {
            get { return driver.ConnectionString; }
            set { driver.ConnectionString = value; }
        }
        /// <summary>
        /// 使用默认数据库连接名称初始化数据库 default
        /// </summary>
        public Database()
        {
            driver = DbDriver.Create();
        }
        /// <summary>
        /// 根据数据库连接名称初始化数据库
        /// </summary>
        /// <param name="name"></param>
        public Database(string name)
        {
            driver = DbDriver.Create(name);
        }
        /// <summary>
        /// 构造连接
        /// </summary>
        /// <returns></returns>
        private DbConnection PrepareConnection()
        {
            if (connection == null) this.connection = driver.CreateConnection();
            return this.connection;
        }
        /// <summary>
        /// 构造数据库操作命令
        /// </summary>
        /// <param name="text">执行语句/存储过程</param>
        /// <param name="type">执行类型</param>
        /// <param name="prms">执行参数</param>
        /// <returns></returns>
        private DbCommand PrepareCommand(string text, CommandType type = CommandType.Text, params Parameter[] prms)
        {
            var command = driver.CreateCommand();
            command.Connection = this.PrepareConnection();
            command.CommandText = text;
            command.CommandType = type;
            if (prms != null)
            {
                foreach (var prm in prms)
                {
                    var parameter = driver.CreateDataParameter();
                    parameter.ParameterName = prm.Name;
                    parameter.Value = prm.Value;
                    parameter.Direction = prm.Direction;
                    command.Parameters.Add(parameter);
                }
            }
            return command;
        }

        /// <summary>
        /// 填充数据集
        /// </summary>
        /// <param name="sql">SQL/存储过程</param>
        /// <param name="type">类型</param>
        /// <param name="prms">执行参数</param>
        /// <returns></returns>
        public DataSet Fill(string sql, CommandType type, params Parameter[] prms)
        {
            Logger.Info(typeof(Database), string.Format("FILL TYPE {0}, SQL:{1}", type, sql));
            try
            {
                using (var command = PrepareCommand(sql, type, prms))
                {
                    var adapter = driver.CreateAdapter();
                    adapter.SelectCommand = command;
                    var dataset = new DataSet();
                    try
                    {
                        adapter.Fill(dataset);
                        FillParameter(command, prms);
                    }
                    finally
                    {
                        var dispose = adapter as IDisposable;
                        if (dispose != null)
                        {
                            dispose.Dispose();
                        }
                    }
                    return dataset;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(typeof(Database), string.Format("FILL TYPE {0}, SQL:{1} ex:{2}", type, sql, ex.ToString()));
                throw;
            }
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="sql">SQL/存储过程</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        public DataSet Pager(string sql, int pageIndex, int pageSize)
        {
            string count = driver.CounterStmt(sql);
            var countData = Fill(count, System.Data.CommandType.Text);
            var total = int.Parse(countData.Tables[0].Rows[0][0].ToString());
            pageIndex = pageIndex < 0 ? 0 : pageIndex;
            int from = pageIndex * pageSize;
            int to = (pageIndex + 1) * pageSize;
            string pager = driver.PagerStmt(sql, pageIndex, pageSize); ;

            var ds = Fill(pager, System.Data.CommandType.Text);
            ds.Tables[0].TableName = "Table";
            var dt = new System.Data.DataTable();
            dt.TableName = "Pager";
            dt.Columns.Add("Count", typeof(int));
            dt.Columns.Add("Total", typeof(int));
            int pageCount = total % pageSize == 0 ? total / pageSize : total / pageSize + 1;
            dt.Rows.Add(total, pageCount);
            ds.Tables.Add(dt);
            return ds;
        }
        /// <summary>
        /// 执行AUD操作语句
        /// </summary>
        /// <param name="sql">SQL/存储过程</param>
        /// <param name="type">执行类型</param>
        /// <param name="prms">参数</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, CommandType type = CommandType.Text, params Parameter[] prms)
        {
            Logger.Info(typeof(Database), string.Format("ExecuteNonQuery TYPE{0}, SQL:{1}", type, sql));
            try
            {
                using (var command = PrepareCommand(sql, type, prms))
                {
                    Open();
                    FillParameter(command, prms);
                    var affected = command.ExecuteNonQuery();
                    Close();
                    return affected;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(typeof(Database), string.Format("ExecuteNonQuery TYPE{0}, SQL:{1} ex:{2}", type, sql, ex.ToString()));
                throw;
            }
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql">SQL/存储过程</param>
        /// <param name="type">执行类型</param>
        /// <param name="prms">执行参数</param>
        /// <returns></returns>
        public static DataSet Query(string sql, CommandType type = CommandType.Text, params Parameter[] prms)
        {
            using (var db = new Database())
            {
                return db.Fill(sql, type, prms);
            }
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="conn">连接节点</param>
        /// <param name="sql">SQL/存储过程</param>
        /// <param name="type">执行类型</param>
        /// <param name="prms">执行参数</param>
        /// <returns></returns>
        public static DataSet Query(string conn, string sql, CommandType type = CommandType.Text, params Parameter[] prms)
        {
            using (var db = new Database(conn))
            {
                return db.Fill(sql, type, prms);
            }
        }

        /// <summary>
        /// 查询数据集并自动释放分配的数据连接
        /// </summary>
        /// <param name="sql">SELECT 查询语句</param>
        /// <param name="dispose">是否自动释放，默认为自动释放</param>
        /// <returns></returns>
        public DataSet QueryAutoRelease(string sql, bool dispose = true)
        {
            try
            {
                var data = this.Fill(sql, CommandType.Text, null);
                return data;
            }
            finally
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// 使用用户名、密码登陆数据库
        /// </summary>
        /// <param name="user">DB用户名</param>
        /// <param name="password">DB密码</param>
        /// <returns>登陆成功 true，否则false</returns>
        public static bool Connect(string user, string password)
        {
            try
            {
                using (Database db = new Database())
                {
                    string[] array = db.ConnectionString.Split(';');

                    var sbd = new StringBuilder();

                    foreach (var section in array)
                    {
                        if (string.IsNullOrEmpty(section)) continue;
                        if (!section.Contains('='))
                        {
                            sbd.AppendFormat("{0};", section);
                            continue;
                        }
                        var pair = section.Split('=');
                        var key = pair[0].ToLower().Trim();
                        switch (key)
                        {
                            case "user id":
                            case "password":
                            case "uid":
                            case "pwd":
                                continue;
                        }
                        sbd.AppendFormat("{0};", section);
                    }

                    sbd.AppendFormat("User Id={0};Password={1};", user, password);
                    db.ConnectionString = sbd.ToString();
                    try
                    {
                        db.PrepareConnection();
                        db.Open(true);
                    }
                    finally
                    {
                        db.Close();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(typeof(Database), string.Format("登录失败{0}/{1}", user, password) + e.Message);
                return false;
            }
        }
        /// <summary>
        /// 构建参数并填充到命令中
        /// </summary>
        /// <param name="command"></param>
        /// <param name="prms"></param>
        private void FillParameter(IDbCommand command, params Parameter[] prms)
        {
            if (prms == null || prms.Length == 0) return;
            foreach (var prm in prms)
            {
                if (prm.Direction == ParameterDirection.Input) continue;
                foreach (IDbDataParameter parameter in command.Parameters)
                {
                    if (parameter.ParameterName == prm.Name)
                    {
                        prm.Value = parameter.Value;
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 执行AUD命令并返回执行结果
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="type">执行类型</param>
        /// <param name="prms">执行参数</param>
        /// <returns></returns>
        public static int ExecuteInt(string sql, CommandType type = CommandType.Text, params Parameter[] prms)
        {
            using (var db = new Database())
            {
                return db.ExecuteNonQuery(sql, type, prms);
            }
        }

        /// <summary>
        /// 尝试打开连接
        /// </summary>
        /// <param name="throwException">产生异常是否抛出 true抛出异常 ，否则不抛出</param>
        void Open(bool throwException = false)
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
            }
            catch
            {
                if (throwException) throw;
            }
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        void Close()
        {
            try
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
            catch { }
        }

        /// <summary>
        /// 释放数据库资源
        /// 关闭并释放连接
        /// </summary>
        public void Dispose()
        {
            if (connection != null)
            {
                connection.Dispose();
                connection = null;
            }
        }
    }
}
