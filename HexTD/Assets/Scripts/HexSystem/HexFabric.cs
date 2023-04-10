using Configs;
using Configs.Constants;
using MapEditor;
using UnityEngine;

namespace HexSystem
{
    public class HexFabric
    {
        private readonly Transform _hexRoot;
        private readonly HexagonPrefabConfig _hexagonPrefabConfig;
        private readonly Layout _layout;

        public HexFabric(HexagonPrefabConfig hexagonPrefabConfig,
            Layout layout)
        {
            _hexagonPrefabConfig = hexagonPrefabConfig;
            _layout = layout;

            _hexRoot = new GameObject("HexRoot").transform;
            _hexRoot.SetAsLastSibling();
            _hexRoot.localPosition = Vector3.zero;
            _hexRoot.localScale = Vector3.one;
        }

        public HexObject CreateHexObject(HexModel hexModel)
        {
            HexObject hexPrefab = GetHexObjectPrefabByName(hexModel.HexType);
            HexObject hexInstance = Object.Instantiate(hexPrefab, _hexRoot);
            hexInstance.SetHex(hexModel.Position);
            hexInstance.transform.position = _layout.ToPlane((Hex3d)hexModel);

            if (hexModel.Data.TryGetValue(HexParamsNameConstants.HexRotationParam, out string rotation))
            {
                var stepNum = int.Parse(rotation);
                hexInstance.transform.Rotate(Vector3.up, MapConstants.AngleStep * stepNum);
            }
            
            return hexInstance;
        }

        private HexObject GetHexObjectPrefabByName(string prefabTypeName)
        {
            switch (prefabTypeName)
            {
                case HexTypeNameConstants.SimpleType:
                    return _hexagonPrefabConfig.SimpleHexObject;
                case HexTypeNameConstants.BridgeType:
                    return _hexagonPrefabConfig.BridgeHexObject;
            }

            return _hexagonPrefabConfig.SimpleHexObject;
        }
    }
}