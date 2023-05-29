using Configs;

namespace Match.Field
{
    public interface IPropsObjectPrefabConfigRetriever
    {
        PropsPrefabConfig.PropsObjectConfig GetPropsByType(string propsTypeName);
    }
}