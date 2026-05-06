using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL.Store
{
    class DbSolver
    {
        List<IMapSolver> solvers;

        public DbSolver()
        {
            solvers = new List<IMapSolver>();
            solvers.Add(new AttributeSolver());
        }

        public MapInfo Interpret(Type type)
        {
            foreach (var solver in solvers)
            {
                try
                {
                    var map = solver.Interpret(type);
                    if (map != null) return map;
                }
                catch (Exception ex)
                {
                    ABL.Logger.Error(this.GetType(), ex);
                }
            }
            return null;
        }
    }
}
