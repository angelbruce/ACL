using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace ABL.Config.Ant
{
    public class AntLoader
    {
        const string IDENTITY_TAG = "name";
        const string TYPE_TAG = "type";
        const string FILE_TAG = "file";

        string antUri = null;

        public AntLoader(string antUri)
        {
            this.antUri = antUri;
        }

        public Dictionary<string, IAnt> Load()
        {
            Dictionary<string, IAnt> hash = new Dictionary<string, IAnt>();
            try
            {
                var document = new XmlDocument();
                document.Load(antUri);
                foreach (XmlNode node in document.DocumentElement.ChildNodes)
                {
                    if (node.NodeType != XmlNodeType.Element) continue;
                    var id = node.Attributes[IDENTITY_TAG].Value;
                    var typeName = node.Attributes[TYPE_TAG].Value;
                    var file = node.Attributes.Cast<XmlAttribute>().Where(d=>d.Name == FILE_TAG).Count() > 0  ?  node.Attributes[FILE_TAG].Value : null;

                    var type = Type.GetType(typeName);
                    IAnt ant = null;
                    ant = type == null ? null : (IAnt)Activator.CreateInstance(type);
                    if (string.IsNullOrEmpty(file))
                    {
                        if (node.ChildNodes.Count == 0) continue;
                        ant.InitItems(node.ChildNodes[0]);
                    }
                    else
                    {
                        var dir = System.IO.Path.GetDirectoryName(antUri);
                        var fileName = System.IO.Path.Combine(dir, file);
                        var refDocument = new XmlDocument();
                        refDocument.Load(fileName);
                        ant.InitItems(refDocument.DocumentElement);
                    }
                    if (hash.ContainsKey(id))
                        throw new Exception(string.Format("ant id:{0} already exists."));
                    hash.Add(id, ant);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return hash;
        }
    }
}
