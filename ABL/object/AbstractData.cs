using System;
using System.IO;
using System.Xml.Serialization;

namespace ABL.Object
{
    [Serializable]
    public abstract class AbstractData
    {
        public AbstractData() { }

        public virtual AbstractData Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer x = new XmlSerializer(this.GetType());
                x.Serialize(stream, this);
                byte[] content = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(content, 0, content.Length);
                stream.Flush();
                stream.Position = 0;
                AbstractData? data = x.Deserialize(stream) as AbstractData;
                if (data == null) throw new Exception("clone fialed");
                return data;
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public EnumEntityState State { get; set; }
    }
}