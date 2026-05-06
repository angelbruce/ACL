using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL.Config.Ant
{
    public interface IAntReslover
    {
        List<IAntItem> Reslove(IAnt ant, System.Xml.XmlNode node);
    }
}
