
namespace ABL.Config.Ant
{
    public interface INameValueAntItem : IAntItem
    {
        string Name { get; set; }
        string Value { get; set; }
    }
}
