using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL.Config.Ant
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AntElementCollectionAttribute : Attribute
    {
        public string Name { get; set; }
        public AntElementCollectionAttribute(string name) { Name = name; }
    }
}
