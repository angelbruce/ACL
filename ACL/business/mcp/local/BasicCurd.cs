using ABL;


namespace ACL.business.mcp.local
{
    public class IDable
    {
        public string Id { get; set; } = string.Empty;
    }


    public class BasicCurd<T> where T : IDable, new()
    {
        public delegate void DgtCreatedHandler(T item);
        public delegate void DgtUpdatedHandler(T item);
        public delegate void DgtDeletedHandler(string id);
        public delegate void DgtCompletedHandler(T item);

        public event DgtCreatedHandler? OnCreated;
        public event DgtUpdatedHandler? OnUpdated;
        public event DgtDeletedHandler? OnDeleted;

        private int nextId = 0;
        protected Dictionary<string, T> datas = new Dictionary<string, T>();


        public T Add(T item, Action<T>? fn)
        {
            T newItem = item.Copy();
            newItem.Id = (nextId++).ToString();
            fn?.Invoke(newItem);
            datas[newItem.Id] = newItem;
            Task.Run(async () => OnCreated?.Invoke(newItem));
            return newItem;
        }

        public T[] Gets()
        {
            return datas.Values.ToArray();
        }

        public T Get(string id)
        {
            if (!datas.ContainsKey(id)) return null;
            return datas[id];
        }

        public T Update(T t, Action<T>? fn)
        {
            var data = Get(t.Id);
            if (data == null) throw new Exception($"data with id {t.Id} does not exist.");

            data.CopyFrom(t);
            fn?.Invoke(data);

            datas[data.Id] = data;

            Task.Run(async () => OnUpdated?.Invoke(data));

            return t;
        }

        public bool Delete(string id)
        {
            var existing = Get(id);
            if (existing == null) return false;

            var removed = datas.Remove(id);
            if (removed)
            {
                Task.Run(async () => OnDeleted?.Invoke(id));
            }

            return removed;
        }
    }
}
