using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL.Config.Ant
{
    [AttributeUsage( AttributeTargets.Class| AttributeTargets.Struct)]
    public class AntRootAttribute:Attribute
    {
        public string Name { get; set; }
        public AntRootAttribute(string name) { this.Name = name; }

    }
}
