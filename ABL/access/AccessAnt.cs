using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABL.Config.Ant;

namespace ABL.Access
{
    /// <summary>
    /// access's ant
    /// </summary>
    public class AccessAnt:BaseAnt
    {
        /// <summary>
        /// create an item of accessant
        /// </summary>
        /// <returns></returns>
        public override IAntItem Create()
        {
            return new AccessAntItem();
        }
    }
}
