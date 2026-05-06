using System;
using System.Runtime.Serialization;
namespace ABL.Object
{
    [Serializable]
    [DataContract]
    public class Item : AbstractData
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Memo { get; set; }
    }
}