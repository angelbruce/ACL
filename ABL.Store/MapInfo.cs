using System.Collections.Generic;
using System;
using System.Linq;
using ABL.Data;

namespace ABL.Store
{
    public class MapInfo
    {
        private TableAttribute table;
        private List<ColumnAttribute> fields;

        public TableAttribute Table
        {
            get { return table; }
            set { table = value; }
        }

        public List<ColumnAttribute> Fields
        {
            get { return fields; }
            set { fields = value; }
        }

        public ColumnAttribute? Pk { get; set; }

        public string ToSelect()
        {
            if (this.Fields.Count <= 0)
                return string.Format("SELECT * FROM {0} ", this.Table.Name);
            else
                return string.Format("SELECT {0} FROM {1} ", String.Join(",", this.Fields.Select(value => string.Format("[{0}]", value.Name)).ToList()), this.Table.Name);
        }

        public string ToDelete()
        {
            return string.Format("DELETE FROM {0} ", this.Table.Name);
        }
    }
}
