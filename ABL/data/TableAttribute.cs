using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL.Data
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class TableAttribute : Attribute
    {
        string name;
        string description;

        public string Name
        {
            get { return name; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public TableAttribute() { }

        public TableAttribute(string name)
        {
            this.name = name;
        }

        public TableAttribute(string name, string modelName)
        {
            this.name = name;
            this.description = modelName;
        }
    }
}
