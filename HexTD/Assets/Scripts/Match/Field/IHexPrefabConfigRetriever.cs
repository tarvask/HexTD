using MapEditor;

namespace Match.Field
{
    public interface IHexPrefabConfigRetriever
    {
        HexObject GetHexByType(string hexTypeName);
    }
}