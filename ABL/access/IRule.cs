using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL.Access
{
    public interface IRule
    {
        IAccessOut Access(IToken token, ILimited limitedItem);
    }
}
