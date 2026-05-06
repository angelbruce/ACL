
namespace ABL.Config.Ant
{
    [AntRoot("config")]
    [AntPrefix("item")]
    public class NameValueAntItem : INameValueAntItem
    {
        [AntElement("name")]
        public string Name { get; set; }

        [AntElement("value")]
        public string Value { get; set; }
    }
}
