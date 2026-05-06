using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ABL.Store
{
    public interface IMapSolver
    {
        MapInfo Interpret(Type type);
    }
}
