using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABL.Config.Ant;
using ABL.Exceptions;

namespace ABL.Access
{
    public class AccessAntContext
    {
        private const string ANT_NAME_TAG = "accessRule";
        private static bool isInitialized = false;
        private static readonly object dummy = new object();
        private static List<AccessAntItem> accessItems = new List<AccessAntItem>();

        /// <summary>
        /// initlaize the uri's context
        /// </summary>
        /// <param name="uri"></param>
        public static void Initialize(string uri)
        {
            if (isInitialized) return;
            lock (dummy)
            {
                if (isInitialized) return;
                var ant = new Ant(uri);
                ant.Load();
                var allItems = ant.Items();
                if (allItems != null && allItems.Count > 0)
                {
                    var items = allItems[ANT_NAME_TAG];
                    if (items != null && items.Count > 0)
                        items.ForEach(item => accessItems.Add(item as AccessAntItem));
                }
                isInitialized = true;
            }
        }

        /// <summary>
        /// get all items of access-rule's configuration
        /// </summary>
        /// <returns></returns>
        public static  List<AccessAntItem>  Items()
        {
            if (!isInitialized)
            {
                throw new ExceptionBase("context's uri does not initialized");
            }
            return accessItems;
        }
    }
}
