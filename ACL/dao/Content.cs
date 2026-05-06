using ABL.Data;
using ABL.Object;
using ABL.Enums;

namespace ACL.dao
{
    [Table("agent_content_store_config")]
    public class ContentStoreConfig : AbstractData
    {
        [Column("agent_content_config_id", IsPK = true, KeyGenerateStrategy = PrimaryKeyGenerateStrategy.AutoIncement)]
        public long Id { get; set; }


        [Column("agent_id")]
        public long AgentId { get; set; } = 0;


        [Column("config_name")]
        public string Name { get; set; } = string.Empty;


        [Column("prefix_test")]
        public string Regex { get; set; } = string.Empty;


        [Column("content_type")]
        public ContentType ContentType { get; set; } = ContentType.Doc;


        [Column("store_type")]
        public StoreType StoreType { get; set; } = StoreType.File;


        [Column("store_dir")]
        public string Dir { get; set; } = string.Empty;


        [Column("store_db_type")]
        public EnumConfigDbType Db { get; set; } = EnumConfigDbType.SQLite;


        [Column("store_db_connectionstring")]
        public string ConnectionString { get; set; } = string.Empty;


        [Column("store_db_insertsql")]
        public string InsertSQL { get; set; } = string.Empty;


        [Column("store_webapi")]
        public string WebApi { get; set; } = string.Empty;
    }

    public class ToBeSupportAttribute : Attribute { }

    public enum EnumConfigDbType
    {
        [Description("None")]
        None,
        [Description("SQLite")]
        SQLite,
        [Description("PG")]
        PG,
        [Description("MySQL")]
        MySQL
    }

    public enum StoreType
    {
        [Description("None")]
        None,
        [Description("File")]
        File,

        [Description("Database")]
        [ToBeSupport]
        Database,

        [Description("WebApi")]
        [ToBeSupport]
        WebApi
    }

    public enum ContentType
    {
        [Description("Yaml")]
        Yaml,
        [Description("Json")]
        Json,
        [Description("Xml")]
        Xml,
        [Description("Doc")]
        Doc,
        [Description("Html")]
        Html
    }
}
