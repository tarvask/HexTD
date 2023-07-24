using System;
using Configs;
using MapEditor;
using Tools;

namespace Match.Field
{
    public class HexPrefabConfigRetriever : BaseDisposable, IHexPrefabConfigRetriever
    {
        private readonly HexagonPrefabConfig _hexagonPrefabConfig;
        
        public HexPrefabConfigRetriever(HexagonPrefabConfig hexagonPrefabConfig)
        {
            _hexagonPrefabConfig = hexagonPrefabConfig;
        }

        public HexObject GetHexByType(string hexTypeName)
        {
            if (!_hexagonPrefabConfig.HexObjects.TryGetValue(hexTypeName, out var hexObject))
            {
                throw new ArgumentException($"Unknown or undefined hex type: {hexTypeName}");
            }
            
            return hexObject;
        }
    }
}