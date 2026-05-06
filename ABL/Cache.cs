using System.Collections.Generic;
namespace ABL
{
    public class Cache<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private static object dummy = new object();

        public void Add(TKey key, TValue value)
        {
            lock (dummy)
            {
                if (!base.ContainsKey(key))
                    base.Add(key, value);
            }
        }

        public TValue Remove(TKey key)
        {
            if (!base.ContainsKey(key))
                return default(TValue);
            lock (dummy)
            {
                if (!this.ContainsKey(key))
                    return default(TValue);
                TValue value = this[key];
                base.Remove(key);
                return value;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (!base.ContainsKey(key)) return default(TValue);
                return base[key];
            }
            set
            {
                if (!base.ContainsKey(key))
                {
                    base.Add(key, value);
                    return;
                }
                base[key] = value;
            }
        }

        public bool Contains(TKey key)
        {
            return base.ContainsKey(key);
        }

        public void Clear()
        {
            lock (dummy)
            {
                base.Clear();
            }
        }

    }
}