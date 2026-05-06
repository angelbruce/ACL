using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABL.Config.Ant;
using System.Reflection;

namespace ABL.Store
{
    public class EntityAsmFinder
    {
        static Ant ant;
        static Assembly assembly;

        public static void Initialize(string uri, string section)
        {
            ant = new Ant(uri);
            ant.Load();
            var items = ant.Items(section);
            if (items == null) return;
            var item = items[0] as NameValueAntItem;
            var asm = item.Name;
            assembly = Assembly.LoadFrom(asm);
        }

        public static Assembly Assembly
        {
            get { return assembly; }
        }

    }
}
