using System;
using Configs;
using Configs.Constants;
using HexSystem;
using MapEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Match.Field
{
    public class HexObjectFabric
    {
        private readonly HexagonPrefabConfig _hexagonPrefabConfig;

        public HexObjectFabric(HexagonPrefabConfig hexagonPrefabConfig)
        {
            _hexagonPrefabConfig = hexagonPrefabConfig;
        }

        public HexObject Create(HexModel hexModel, Transform root, Vector3 position)
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

        
        //#85925650 дублирование
        private HexObject GetHexObjectPrefabByName(string hexTypeName)
        {
            
            if (!_hexagonPrefabConfig.HexObjects.TryGetValue(hexTypeName, out var hexObject))
            {
                throw new ArgumentException($"Unknown or undefined cell type {hexTypeName}");
            }
            
            return hexObject;
        }
    }
}