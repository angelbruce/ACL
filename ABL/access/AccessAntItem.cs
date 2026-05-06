using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABL.Config.Ant;

namespace ABL.Access
{
    /// <summary>
    /// access's ant item
    /// </summary>
    [AntPrefix("access")]
    [AntRoot("accessCollection")]
    public class AccessAntItem:IAntItem
    {
        /// <summary>
        /// rule type
        /// </summary>
        [AntElement("rule")]
        public string Rule { get; set; }
        /// <summary>
        /// token type
        /// </summary>
        [AntElement("token")]
        public string Token { get; set; }
    }
}
