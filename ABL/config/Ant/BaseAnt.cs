using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL.Config.Ant
{
    [AntResolve(typeof(AntResolver))]
    public abstract class BaseAnt : IAnt
    {
        private System.Xml.XmlNode node = null;
        private List<IAntItem> items = null;

        public virtual List<IAntItem> GetItems()
        {
            if (items == null)
            {
                items = AntResolverContext.Create(this, node);
            }
            return items;
        }

        public abstract IAntItem Create();

        void IAnt.InitItems(System.Xml.XmlNode node)
        {
            this.node = node;
        }
    }
}
