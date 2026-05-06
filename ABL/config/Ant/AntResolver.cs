using System;
using System.Collections.Generic;
using System.Linq;

namespace ABL.Config.Ant
{
    class AntResolver : IAntReslover
    {
        public List<IAntItem> Reslove(IAnt ant, System.Xml.XmlNode node)
        {
            if (ant == null)
                throw new ArgumentNullException("ant", "instance of IAnt cannot be null");
            if (node == null)
                return null;
            var instance = ant.Create();
            if (instance == null)
                throw new Exception("ant create return's null");
            var type = instance.GetType();
            var rootAttributes = type.GetCustomAttributes(typeof(AntRootAttribute), true);
            if (rootAttributes == null)
                throw new Exception(string.Format("{0}' root attribute cannot be fixed", type.FullName));
            var root = (rootAttributes[0] as AntRootAttribute)?.Name;
            if (string.IsNullOrEmpty(root))
                throw new Exception(string.Format("{0}'s root cannot be null or empty", type.FullName));
            if (root != node.Name)
                throw new Exception(string.Format("{0}'s no root is {1},but the node's prefix is {2}", type.FullName, root, node.Name));
            var prefixAttributes = type.GetCustomAttributes(typeof(AntPrefixAttribute), true);
            if (prefixAttributes == null)
                throw new Exception(string.Format("{0}'  prefix attribute cannot be fixed", type.FullName));
            var prefix = (prefixAttributes[0] as AntPrefixAttribute)?.Name;
            if (string.IsNullOrEmpty(prefix))
                throw new Exception(string.Format("{0}'s prefix cannot be null or empty", type.FullName));
            var elementHash = new Dictionary<string, System.Reflection.PropertyInfo>();
            var elementCollectionHash = new Dictionary<string, System.Reflection.PropertyInfo>();
            foreach (var property in type.GetProperties())
            {
                var elementAttributes = property.GetCustomAttributes(typeof(AntElementAttribute), true);
                if (elementAttributes.Count() > 0)
                {
                    elementHash.Add(((AntElementAttribute)elementAttributes[0]).Name, property);
                    continue;
                }
                var elementCollectionAttributes = property.GetCustomAttributes(typeof(AntElementCollectionAttribute), true);
                if (elementCollectionAttributes.Length > 0) elementCollectionHash.Add(((AntElementCollectionAttribute)elementCollectionAttributes[0]).Name, property);
            }
            var itemList = new List<IAntItem>();
            foreach (System.Xml.XmlNode child in node.ChildNodes)
            {
                if (child.NodeType != System.Xml.XmlNodeType.Element) continue;
                if (prefix != child.Name)
                    throw new Exception(string.Format("{0}'s no prefix is {1},but the node's prefix is {2}", type.FullName, prefix, child.Name));
                var item = ant.Create();
                itemList.Add(item);
                foreach (var pair in elementHash)
                    SetValue(item, pair.Value.Name, child, pair.Key);
                if (elementCollectionHash.Count <= 0 || child.ChildNodes.Count <= 0) continue;
                var set = from ele in elementCollectionHash
                          from xml in child.ChildNodes.Cast<System.Xml.XmlNode>()
                          where ele.Key == xml.Name
                          select new { Property = ele.Value, Type = ele.Value.PropertyType.GetGenericArguments()[0], XmlNode = xml };
                foreach (var eNode in set.ToList())
                {
                    var innerList = Activator.CreateInstance(eNode.Property.PropertyType);
                    Reslove(innerList, eNode.Type, eNode.XmlNode);
                    item.Set(eNode.Property.Name, innerList);
                }
            }
            return itemList;
        }

        void Reslove(object itemList, Type type, System.Xml.XmlNode node)
        {
            if (node == null) return;
            var prefixAttributes = type.GetCustomAttributes(typeof(AntPrefixAttribute), true);
            if (prefixAttributes == null)
                throw new Exception(string.Format("{0}'  prefix attribute cannot be fixed", type.FullName));
            var prefix = (prefixAttributes[0] as AntPrefixAttribute).Name;
            if (string.IsNullOrEmpty(prefix))
                throw new Exception(string.Format("{0}'s prefix cannot be null or empty", type.FullName));
            var elementHash = new Dictionary<string, System.Reflection.PropertyInfo>();
            var elementCollectionHash = new Dictionary<string, System.Reflection.PropertyInfo>();
            foreach (var property in type.GetProperties())
            {
                var elementAttributes = property.GetCustomAttributes(typeof(AntElementAttribute), true);
                if (elementAttributes.Count() > 0) { elementHash.Add(((AntElementAttribute)elementAttributes[0]).Name, property); continue; }
                var elementCollectionAttributes = property.GetCustomAttributes(typeof(AntElementCollectionAttribute), true);
                if (elementCollectionAttributes.Length > 0)
                    elementCollectionHash.Add(((AntElementCollectionAttribute)elementCollectionAttributes[0]).Name, property);
            }
            foreach (System.Xml.XmlNode child in node.ChildNodes)
            {
                if (child.NodeType != System.Xml.XmlNodeType.Element) continue;
                if (prefix != child.Name)
                    throw new Exception(string.Format("{0}'s no prefix is {1},but the node's prefix is {2}", type.FullName, prefix, child.Name));
                var item = Activator.CreateInstance(type) as IAntItem;
                itemList.Invoke("Add", new object[] { item });
                foreach (var pair in elementHash)
                    SetValue(item, pair.Value.Name, child, pair.Key);
                if (elementCollectionHash.Count <= 0 || child.ChildNodes.Count <= 0) continue;
                var set = from ele in elementCollectionHash
                          from xml in child.ChildNodes.Cast<System.Xml.XmlNode>()
                          where ele.Key == xml.Name
                          select new { Property = ele.Value, Type = ele.Value.PropertyType.GetGenericArguments()[0], XmlNode = xml };
                foreach (var eNode in set.ToList())
                {
                    var innerList = Activator.CreateInstance(eNode.Property.PropertyType);
                    Reslove(innerList, eNode.Type, eNode.XmlNode);
                    item.Set(eNode.Property.Name, innerList);
                }
            }
        }

        void SetValue(IAntItem item, string propName, System.Xml.XmlNode node, string name)
        {
            var attributes = node.Attributes.Cast<System.Xml.XmlAttribute>().Where(d => d.Name == name).ToList();
            if (attributes.Count > 0)
            {
                item.Set(propName, attributes.First().Value);
                return;
            }
            var nodeSet = node.ChildNodes.Cast<System.Xml.XmlNode>().Where(d => d.Name == name).ToList();
            if (nodeSet.Count > 0)
                item.Set(propName, nodeSet.First().InnerText);
        }
    }
}



