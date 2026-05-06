using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        string name;
        string propertyName;
        Type type;

        public string Name
        {
            set { name = value; }
            get { return name; }
        }
        public string PropertyName
        {
            get { return propertyName; }
            set { propertyName = value; }
        }
        public Type Type
        {
            get { return this.type; }
            set { this.type = value; }
        }

        public string Description { get; set; } = string.Empty;
        public bool IsPK { get; set; } = false;
        public PrimaryKeyGenerateStrategy KeyGenerateStrategy { get; set; } = PrimaryKeyGenerateStrategy.None;

        public ColumnAttribute() { }

        public ColumnAttribute(string name)
        {
            this.name = name;
        }

        public ColumnAttribute(string name, string propertyName)
        {
            this.name = name;
            this.propertyName = propertyName;
        }

    }

}
