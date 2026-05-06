using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABL;
using ABL.Exceptions;

namespace ABL.Access
{
    /// <summary>
    /// access' context
    /// </summary>
    public abstract class Context
    {
        /// <summary>
        /// get the result of limited by token 
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="limited">limited</param>
        /// <param name="bubble">bubble</param>
        /// <returns></returns>
        public static IAccessOut Access(IToken token, ILimited limited, bool bubble = false)
        {
            var items = AccessAntContext.Items();
            if (items == null)
                throw new ExceptionBase("access-rule'configuration dose not initialized,please check and correct it");
            var set = items.Where(d => d.Token == token.GetType().FullName).ToList();
            if (!set.Any())
                throw new ExceptionBase(string.Format("token {0} of access-rule does not exist",
                                                      token.GetType().FullName));
            var type = Type.GetType(set.First().Rule);
            if (type == null)
                throw new ExceptionBase(string.Format("type {0} of access-rule does not exist", token.GetType().FullName));
            var rule = Activator.CreateInstance(type) as IRule;
            if (rule == null)
                throw new ExceptionBase(string.Format("rule {0} cannot be newed from constructed",
                                                      token.GetType().FullName));
            return rule.Access(token, limited);
        }


    }
}
