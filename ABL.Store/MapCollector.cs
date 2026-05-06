using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ABL;
using ABL.Exceptions;

namespace ABL.Store
{
    class MapCollector
    {
        static System.Threading.ReaderWriterLockSlim slim;
        static Dictionary<Type, MapInfo> mapCaches;
        static DbSolver solver;

        static MapCollector()
        {
            slim = new System.Threading.ReaderWriterLockSlim();
            mapCaches = new Dictionary<Type, MapInfo>();
            solver = new DbSolver();
        }

        public static MapInfo Get(Type type)
        {
            if (!mapCaches.ContainsKey(type)) Register(type);
            try
            {
                slim.EnterReadLock();
                return mapCaches[type];
            }
            finally
            {
                slim.ExitReadLock();
            }
        }

        private static void Register(Type type)
        {
            try
            {
                slim.EnterWriteLock();
                if (mapCaches.ContainsKey(type)) return;
                var map = solver.Interpret(type);
                if (map == null)
                    throw new ExceptionBase(string.Format("cannot found the map of {0} from entity or attribute,please check", type.Name));
                mapCaches.Add(type, map);
            }
            finally
            {
                slim.ExitWriteLock();
            }
        }
    }
}
