using Configs;
using Configs.Constants;
using MapEditor;
using UnityEngine;

namespace HexSystem
{
    public class HexFabric
    {
        private readonly HexagonPrefabConfig _hexagonPrefabConfig;

        public HexFabric(HexagonPrefabConfig hexagonPrefabConfig)
        {
            _hexagonPrefabConfig = hexagonPrefabConfig;
        }

        public HexObject CreateHexObject(HexModel hexModel, Transform root, Vector3 position)
        {
            HexObject hexPrefab = GetHexObjectPrefabByName(hexModel.HexType);
            HexObject hexInstance = Object.Instantiate(hexPrefab, root);
            hexInstance.SetHex(hexModel.Position);
            hexInstance.transform.position = position; //_layout.ToPlane((Hex3d)hexModel);
            hexInstance.gameObject.name = hexModel.Position.ToString();

            if (hexModel.Data.TryGetValue(HexParamsNameConstants.HexRotationParam, out string rotation))
            {
                var stepNum = int.Parse(rotation);
                hexInstance.transform.Rotate(Vector3.up, MapConstants.AngleStep * stepNum);
            }
            
            if (hexModel.Data.TryGetValue(HexParamsNameConstants.IsBlockerParam, out string isBlocker))
            {
                hexInstance.SetIsBlocker(bool.Parse(isBlocker));
            }
            else
            {
                hexInstance.SetIsBlocker(false);
            }
            
            hexInstance.SetIsHighlighted(false);
            
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