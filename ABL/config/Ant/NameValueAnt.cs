using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL.Config.Ant
{
    public class NameValueAnt : BaseAnt
    {
        public override IAntItem Create()
        {
            return new NameValueAntItem();
        }
    }
}
