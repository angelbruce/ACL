using ABL.Config.Ant;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ABL.config.Ant
{
    /// <summary>
    /// and default store which store data to xml file,
    /// the structure of xml file is defined by the attribute of item type, and the data of items is stored in 
    /// xml file according to the structure.
    /// </summary>
    public class AntDefaultStore : IDisposable
    {
        private Type antType;
        private string uri;
        private bool createIfNotExists;
        private string name;
        private XmlDocument document;

        /// <summary>
        ///  construction
        /// </summary>
        /// <param name="antType"></param>
        /// <param name="uri"></param>
        /// <param name="name"></param>
        /// <param name="createIfNotExists"></param>
        public AntDefaultStore(Type antType, string uri, string name, bool createIfNotExists)
        {
            this.antType = antType;
            this.uri = uri;
            this.name = name;
            this.createIfNotExists = createIfNotExists;
            document = new XmlDocument();
        }

        /// <summary>
        /// initialize the store, if the uri does not exist, create a new one if createIfNotExists is true, otherwise throw exception
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Initialize()
        {
            if (!File.Exists(uri))
            {
                if (createIfNotExists)
                {
                    var dir = Path.GetDirectoryName(uri);
                    if (dir == null || dir.Length == 0)
                    {
                        throw new Exception($"invalid uri: {uri}");
                    }

                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    var defaultContent = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><configuration></configuration>";
                    var buffer = Encoding.UTF8.GetBytes(defaultContent);
                    using var stream = File.Create(uri);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Close();
                }
            }
        }

        /// <summary>
        /// save all items to uri belongs to the ant node 
        /// </summary>
        public void Save()
        {
            document.Load(uri);

            var items = AntContext.Instance.GetItems(name);
            //get or create ant node if not exists, ant node is the declaration of the data-items which contains the storage structure of all data-items.
            var antNode = GetOrCreateAntNode(document, name, antType);

            WriteItems(antNode, items);

            document.Save(uri);
        }

        private void WriteItems(XmlNode antNode, List<IAntItem> items)
        {
            // clear existing items
            var nodes = antNode.ChildNodes;
            foreach (var nd in nodes)
            {
                if (nd is XmlNode node)
                {
                    antNode.RemoveChild(node);
                }
            }

            if (items == null || items.Count == 0) return;

            //get the prefix & root of the item type
            var type = items[0].GetType();
            var antPrefixAttr = type.GetCustomAttribute<AntPrefixAttribute>();
            var antRootAttr = type.GetCustomAttribute<AntRootAttribute>();
            if (antPrefixAttr == null || antRootAttr == null) return;

            //check the prefix & root is valid
            var antPrefix = antPrefixAttr.Name;
            var antRoot = antRootAttr.Name;
            if (string.IsNullOrEmpty(antPrefix) || string.IsNullOrEmpty(antRoot)) return;


            // create collection node before create items, collection is the set of items,
            // and item is the element of collection
            var collection = document.CreateElement(antRoot);
            antNode.AppendChild(collection);

            CreateElementNode(collection, antPrefix, items);
        }

        /// <summary>
        /// store all items data into collection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="prefix"></param>
        /// <param name="items"></param>
        private void CreateElementNode(XmlNode collection, string prefix, List<IAntItem> items)
        {
            if (items == null || items.Count == 0) return;
            //get type from first item, and get the relation of property and element/collection defined by attribute,
            //the relation is used to store element value and collection value of item into xml node
            var type = items[0].GetType();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var qry = from p in props
                      let elementAttr = p.GetCustomAttribute<AntElementAttribute>()
                      let collectionAttr = p.GetCustomAttribute<AntElementCollectionAttribute>()
                      where elementAttr != null || collectionAttr != null
                      select new
                      {
                          Prop = p,
                          Element = elementAttr,
                          Collection = collectionAttr,
                          IsCollection = collectionAttr != null,
                          IsElement = elementAttr != null
                      };

            var relationList = qry.ToList();
            //store the item one by one
            foreach (var item in items)
            {
                var itemNode = document.CreateElement(prefix);
                collection.AppendChild(itemNode);
                //store the element value and collection value of the item according the relation defined by the attribute,
                //element is stored as attribute of item node, and collection is stored as child node of item node
                foreach (var relation in relationList)
                {
                    var value = relation.Prop.GetValue(item);
                    if (value == null) continue;

                    //store element value as attribute of item node
                    if (relation.IsElement)
                    {
                        var val = value == null ? string.Empty : value.ParseTo<string>();
                        var name = relation.Element.Name;
                        var attr = document.CreateAttribute(name);
                        attr.Value = val;
                        itemNode.Attributes.Append(attr);
                    }
                    //store collection value as child node of item node
                    else if (relation.IsCollection)
                    {
                        var subItems = value as List<IAntItem>;
                        if (subItems == null || subItems.Count == 0)
                        {
                            continue;
                        }


                        //get the prefix of sub item type, the prefix is defined by the ant-prefix-attribute of sub item type,
                        //and recursive create element node for sub items
                        var subType = subItems[0].GetType();
                        var antPrefixAttr = subType.GetCustomAttribute<AntPrefixAttribute>();
                        if (antPrefixAttr == null) continue;

                        //check the prefix is valid
                        var antPrefix = antPrefixAttr.Name;
                        if (string.IsNullOrEmpty(antPrefix)) return;

                        //create sub collection node ,the name of sub collection node is defined by the element-collection-attribute
                        var collectionNode = document.CreateElement(relation.Collection.Name);

                        //recursive create element node for sub items
                        CreateElementNode(collectionNode, antPrefix, subItems);

                        itemNode.AppendChild(collectionNode);
                    }
                }
            }
        }

        /// <summary>
        /// get or create if not exists ant node which stores concret items 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <param name="antType"></param>
        /// <returns></returns>
        private XmlNode GetOrCreateAntNode(XmlDocument document, string name, Type antType)
        {
            var doc = document.DocumentElement;
            if (doc == null)
            {
                doc = document.CreateElement("configuration");
                document.AppendChild(doc);
            }

            XmlNode? node = null;
            if (doc.ChildNodes != null && doc.ChildNodes.Count > 0)
            {
                node = doc.SelectSingleNode($"./*[@name='{name}']");
            }

            if (node == null)
            {
                node = document.CreateElement("antConfig");
                doc.AppendChild(node);

                var nameAttr = document.CreateAttribute("name");
                nameAttr.Value = name;
                node.Attributes.Append(nameAttr);

                var typeAttr = document.CreateAttribute("type");
                typeAttr.Value = antType.AssemblyQualifiedName;
                node.Attributes.Append(typeAttr);

            }

            return node;
        }

        /// <summary>
        /// dispose the document
        /// </summary>
        public void Dispose()
        {
            if (document != null)
            {
                document = null;
            }
        }
    }
}
