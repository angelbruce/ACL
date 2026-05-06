using System.Collections.Generic;
using System;

namespace ABL.Config.Ant
{
    /// <summary>
    /// ant class
    /// you have to load and then use items
    /// </summary>
    public class Ant
    {
        AntLoader loader = null;
        Dictionary<string, List<IAntItem>> antItems = new Dictionary<string, List<IAntItem>>();
        string uri = null;

        /// <summary>
        /// construct ant from uri
        /// </summary>
        /// <param name="uri"></param>
        public Ant(string uri)
        {
            this.uri = uri;
            if (string.IsNullOrEmpty(uri))
                throw new Exception("uri cannot be null or empty");
            loader = new AntLoader(uri);
        }

        /// <summary>
        /// get named items
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<IAntItem> Items(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("name cannot be null or empty");
            }

            if (!antItems.TryGetValue(name, out var items))
            {
                throw new KeyNotFoundException($"Item group '{name}' not found in the loaded Ant structure.");
            }

            return antItems[name];
        }

        /// <summary>
        /// get all items
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<IAntItem>> Items()
        {
            return antItems;
        }

        /// <summary>
        /// load items from uri 
        /// </summary>
        public void Load()
        {
            antItems.Clear();
            foreach ((string key, IAnt value) in loader.Load())
            {
                if (value == null) continue;

                antItems.Add(key, value.GetItems());
            }
        }
    }
}

