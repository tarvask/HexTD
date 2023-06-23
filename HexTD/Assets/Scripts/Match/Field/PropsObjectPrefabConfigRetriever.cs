using System;
using Configs;
using Tools;

namespace Match.Field
{
    public class PropsObjectPrefabConfigRetriever : BaseDisposable, IPropsObjectPrefabConfigRetriever
    {
        private readonly PropsPrefabConfig _propsPrefabConfig;
        
        public PropsObjectPrefabConfigRetriever(PropsPrefabConfig propsPrefabConfig)
        {
            _propsPrefabConfig = propsPrefabConfig;
        }

        public PropsPrefabConfig.PropsObjectConfig GetPropsByType(string propsTypeName)
        {
            if (!_propsPrefabConfig.PropsObjectConfigs.TryGetValue(propsTypeName, out var propsObject))
            {
                throw new ArgumentException("Unknown or undefined props type");
            }
            
            return propsObject;
        }
    }
}