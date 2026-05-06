using System;
using System.Collections.Generic;
using System.Linq;

namespace ABL.Config.Ant
{
    class AntResolverContext
    {
        public static List<IAntItem> Create(IAnt ant, System.Xml.XmlNode node)
        {
            if (ant == null)
                throw new ArgumentNullException("ant", "instance of IAnt cannot be null");
            var attributes = ant.GetType().GetCustomAttributes(typeof(AntResolveAttribute), true);
            if (attributes == null || attributes.Count() <= 0)
                throw new Exception("ant's resolve-attribute is null");
            var type = ((AntResolveAttribute)attributes[0]).Resolver;
            var resolver = Activator.CreateInstance(type) as IAntReslover;
            return resolver.Reslove(ant, node);
        }
    }
}
