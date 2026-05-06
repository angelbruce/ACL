using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL.Config.Ant
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class AntResolveAttribute : Attribute
    {
        public Type Resolver { get; set; }
        public AntResolveAttribute(Type resolvor)
        {
            this.Resolver = resolvor;
        }
    }
}
