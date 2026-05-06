
namespace ABL.Store
{
    public interface IDbSchema
    {
        bool Check(string table);

        void Create(MapInfo map);
    }
}
