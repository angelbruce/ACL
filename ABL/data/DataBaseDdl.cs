using ABL.Object;
using System;
using System.Collections.Generic;
using System.Text;

namespace ABL.Data
{
    [Table]
    public class Schema : AbstractData
    {
        [Column("column_name")]
        public string Name { get; set; } = string.Empty;
        public List<Table> Tables { get; set; } = new List<Table>();
    }

    [Table]
    public class Table : AbstractData
    {
        [Column("table_name")]
        public string Name { get; set; } = string.Empty;

        [Column("table_comment")]
        public string Description { get; set; } = string.Empty;

        public List<Column> Columns { get; set; } = new List<Column>();
    }

    [Table]
    public class Column : AbstractData
    {
        [Column("column_name")]
        public string Name { get; set; } = string.Empty;

        [Column("column_comment")]
        public string Description { get; set; } = string.Empty;

        [Column("pk")]
        public bool IsPK { get; set; } = false;

        [Column("column_datatype")]
        public string DataType { get; set; } = string.Empty;

        public Type? Type { get; set; }
        public PrimaryKeyGenerateStrategy KeyGenerateStrategy { get; set; } = PrimaryKeyGenerateStrategy.None;
    }

    public enum PrimaryKeyGenerateStrategy
    {
        None,
        AutoIncement,
        UUID
    }


}
