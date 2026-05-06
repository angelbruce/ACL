using ABL.Config.Ant;
using System;
using System.Collections.Generic;
using System.Text;

namespace ABL.Data
{

    public class DbDriverConfigAnt : BaseAnt
    {
        public const string DATABASE_TAG = "databaseDriver";
        public override IAntItem Create()
        {
            return new DbDriverConfig();
        }
    }

    [AntRoot("databaseCollection")]
    [AntPrefix("database")]
    public class DbDriverConfig : IAntItem
    {
        [AntElement("name")]
        public string Name { get; set; } = string.Empty;


        [AntElement("provider")]
        public string Provider { get; set; } = string.Empty;


        [AntElement("connectionString")]
        public string ConnectionString { get; set; } = string.Empty;
    }
}
