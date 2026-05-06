using ABL.Config.Ant;
using System.Data;
using System.Data.Common;

namespace ABL.Data
{
    /// <summary>
    /// 数据库执行参数
    /// </summary>
    public class Parameter
    {
        public Parameter()
        {
            this.Direction = ParameterDirection.InputOutput;
        }
        /// <summary>
        /// 构造数据库执行参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        public Parameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
            this.Direction = ParameterDirection.Input;
        }
        /// <summary>
        /// 构造数据库执行参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="direction">参数方向</param>
        public Parameter(string name, object value, ParameterDirection direction)
        {
            this.Name = name;
            this.Value = value;
            this.Direction = direction;
        }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// 参数方向
        /// </summary>
        public ParameterDirection Direction { get; set; }
    }

    /// <summary>
    /// 数据库实例句柄构建工厂
    /// </summary>
    public abstract class DbDriver
    {
        /// <summary>
        /// 数据库连接串
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <returns></returns>
        public abstract DbConnection CreateConnection();
        /// <summary>
        /// 创建数据库执行命令
        /// </summary>
        /// <returns></returns>
        public abstract DbCommand CreateCommand();
        /// <summary>
        /// 创建数据库执行命令参数
        /// </summary>
        /// <returns></returns>
        public abstract DbParameter CreateDataParameter();
        /// <summary>
        /// 创建数据库查询适配器
        /// </summary>
        /// <returns></returns>
        public abstract DbDataAdapter CreateAdapter();
        /// <summary>
        /// 创建command builder.
        /// </summary>
        /// <returns></returns>
        public abstract DbCommandBuilder CreateCommandBuilder();
        /// <summary>
        /// 构建当前时间查询语句
        /// </summary>
        /// <returns></returns>
        public abstract string NowStmt();
        /// <summary>
        /// 构建分页查询语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="index">页面索引</param>
        /// <param name="size">页面大小</param>
        /// <returns></returns>
        public abstract string PagerStmt(string sql, int index, int size);
        /// <summary>
        /// 构建记录总数查询语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        public abstract string CounterStmt(string sql);

        /// <summary>
        /// 根据数据类型获取数据库类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract string GetDataType(Type type);
        /// <summary>
        /// 创建schema DDL
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract string CreateSchemaDDL(Schema schema);
        /// <summary>
        /// table DDL
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract List<string> CreateTableDDL(Table table);
        /// <summary>
        /// 获取schemas
        /// </summary>
        /// <param name="data">extended data</param>
        /// <returns></returns>
        public abstract List<Schema> FetchSchemas(object data);

        /// <summary>
        /// 根据数据库连接节点创建数据实例句柄构建工厂
        /// </summary>
        /// <param name="name">数据库连接节点</param>
        /// <returns></returns>
        public static DbDriver Create(string name = "default")
        {
            var items = AntContext.Instance.GetItems(DbDriverConfigAnt.DATABASE_TAG);
            if (items == null || items.Count == 0) throw new Exception("database config needed.");
            DbDriverConfig? dbConfig = null;

            foreach (var item in items)
            {
                if (item.GetType() != typeof(DbDriverConfig)) continue;
                var config = (DbDriverConfig)item;
                var dbConfigName = config.Name;
                if (dbConfigName == name)
                {
                    dbConfig = config;
                    break;
                }
            }

            if (dbConfig == null) throw new Exception($"the database of {name} does not exist.");

            var provider = dbConfig.Provider;
            if (provider == null || provider.Length == 0) throw new Exception($"the provider of database {name} need to be config");

            var type = Type.GetType(provider);
            if (type == null) throw new Exception($"invalied provider or database {name}");

            var driver = Activator.CreateInstance(type) as DbDriver;
            if (driver == null) throw new Exception($"cannot create instance of the provider which database is {name}");

            driver.ConnectionString = dbConfig.ConnectionString;
            return driver;
        }
    }

}
