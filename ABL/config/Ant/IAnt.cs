using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL.Config.Ant
{
    public interface IAnt
    {
        List<IAntItem> GetItems();
        void InitItems(System.Xml.XmlNode node);
        IAntItem Create();
    }
}
