using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL.Config.Ant
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AntElementAttribute : Attribute
    {
        public string Name { get; set; }
        public AntElementAttribute(string name) { Name = name; }
    }
}
