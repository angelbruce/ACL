using ABL.Object;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ABL.Store
{
    public abstract class DataStateChecker<O, N, T, K>
        where O : class
        where N : class
        where T : class
    {

        public List<T> Compare(IEnumerable<O> raws, IEnumerable<N> news)
        {
            var rawMap = raws.ToDictionary(GetOKey, y => y);
            var newMap = news.ToDictionary(GetNKey, y => y);
            var list = new List<T>();

            foreach (var nwe in news)
            {
                var key = GetNKey(nwe);
                var t = NCast2T(nwe);
                if (!rawMap.ContainsKey(key))
                {
                    ChangeState(t, EnumEntityState.Added);
                }
                else
                {
                    ChangeState(t, EnumEntityState.Modified);
                }

                list.Add(t);
            }

            foreach (var raw in raws)
            {
                var key = GetOKey(raw);
                if (!newMap.ContainsKey(key))
                {
                    var t = OCast2T(raw);
                    ChangeState(t, EnumEntityState.Deleted);
                    list.Add(t);
                }
            }

            return list;
        }

        protected abstract K GetOKey(O t);
        protected abstract K GetNKey(N t);

        protected abstract T OCast2T(O o);
        protected abstract T NCast2T(N n);

        protected abstract void ChangeState(T v, EnumEntityState state);
    }


    public abstract class AbstractDataStateChecker<O, N, T, K> : DataStateChecker<O, N, T, K>
    where O : AbstractData
    where N : AbstractData
    where T : AbstractData
    {

        protected override void ChangeState(T t, EnumEntityState state)
        {
            t.State = state;
        }
    }

    public abstract class AbstractDataStateChecker<O, N, K> : DataStateChecker<O, N, N, K>
        where O : AbstractData
        where N : AbstractData
    {
        protected override N NCast2T(N n) { return n; }

        protected override void ChangeState(N t, EnumEntityState state)
        {
            t.State = state;
        }
    }

    public abstract class AbstractDataStateChecker<O, K> : DataStateChecker<O, O, O, K>
        where O : AbstractData
    {
        protected abstract K GetKey(O o);

        protected override K GetOKey(O t)
        {
            return GetKey(t);
        }

        protected override K GetNKey(O t)
        {
            return GetKey(t);
        }

        protected override O OCast2T(O o) { return o; }
        protected override O NCast2T(O n) { return n; }

        protected override void ChangeState(O t, EnumEntityState state)
        {
            t.State = state;
        }
    }

}
