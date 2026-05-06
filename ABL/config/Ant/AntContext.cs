using ABL.config.Ant;
using System;
using System.Collections.Generic;

namespace ABL.Config.Ant
{
    /// <summary>
    /// antCollection-ant[name,class,file]
    /// </summary>
    public class AntContext
    {
        private static readonly object dummy = new object();
        private static AntContext context = null;

        Cache<string, List<string>> cacheKeys = new Cache<string, List<string>>();
        Cache<string, List<IAntItem>> cacheItems = new Cache<string, List<IAntItem>>();

        /// <summary>
        /// cannot be initlaized by other class and cannot be inherited.
        /// </summary>
        private AntContext() { }

        /// <summary>
        /// context's instance
        /// </summary>
        public static AntContext Instance
        {
            get
            {
                if (context == null)
                    lock (dummy)
                        if (context == null)
                            context = new AntContext();
                return context;
            }
        }

        /// <summary>
        /// get named items
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<IAntItem> GetItems(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("name cannot be null or empty");
            return cacheItems[name];
        }


        /// <summary>
        /// set item with names
        /// </summary>
        /// <param name="name"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool SetItems(Type antType, string name, List<IAntItem> items)
        {
            SetItems(antType, name, items, false);
            return true;
        }


        /// <summary>
        /// set item with names
        /// </summary>
        /// <param name="name"></param>
        /// <param name="items"></param>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool SetItems(Type antType, string name, List<IAntItem> items, bool createIfNotExists)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("name cannot be null or empty");

            cacheItems[name] = items;
            FlushAsync(antType, name, createIfNotExists);
            return true;
        }

        /// <summary>
        /// reload the uri
        /// </summary>
        /// <param name="uri"></param>
        public void Refresh(string uri)
        {
            UnRegisger(uri);
            Register(uri);
        }

        /// <summary>
        /// register uri's items
        /// </summary>
        /// <param name="uri"></param>
        public void Register(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                return;
            Ant ant = new Ant(uri);
            ant.Load();
            List<string> keys = new List<string>();
            foreach (var item in ant.Items())
            {
                keys.Add(item.Key);
                cacheItems.Add(item.Key, item.Value);
            }
            cacheKeys.Add(uri, keys);
        }

        /// <summary>
        /// unregister uri's items
        /// </summary>
        /// <param name="uri"></param>
        public void UnRegisger(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new Exception("uri cannot be null or empty");
            foreach (var key in cacheKeys[uri])
                cacheItems.Remove(key);
        }

        /// <summary>
        ///  flush the items to file or other storage
        /// </summary>
        /// <param name="antType"></param>
        /// <param name="name"></param>
        /// <param name="createIfNotExists"></param>
        private void FlushAsync(Type antType, string name, bool createIfNotExists)
        {
            if (antType == null)
                throw new Exception("antType cannot be null");

            if (name == null)
                throw new Exception("name cannot be null");

            Task.Run(() =>
            {
                var uri = string.Empty;
                foreach (var item in cacheKeys)
                {
                    uri = item.Key;
                    if (item.Value.Contains(name))
                    {
                        uri = item.Key;
                        break;
                    }
                }

                if (uri == string.Empty)
                {
                    return;
                }

                var store = new AntDefaultStore(antType, uri, name, createIfNotExists);
                store.Initialize();
                store.Save();
            });
        }
    }
}
