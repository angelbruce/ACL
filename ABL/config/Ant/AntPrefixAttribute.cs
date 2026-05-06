using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL.Config.Ant
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class AntPrefixAttribute : Attribute
    {
        public string Name { get; set; }
        public AntPrefixAttribute(string name)
        {
            Name = name;
        }
    }
}
