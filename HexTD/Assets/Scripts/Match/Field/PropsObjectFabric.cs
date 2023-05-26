using System;
using Configs;
using Configs.Constants;
using MapEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Match.Field
{
	public class PropsObjectFabric
    {
        private readonly PropsPrefabConfig _propsPrefabConfig;

        public PropsObjectFabric(PropsPrefabConfig propsPrefabConfig)
        {
            _propsPrefabConfig = propsPrefabConfig;
        }

        public PropsObject Create(PropsModel model, Transform root, Vector3 position)
        {
            PropsPrefabConfig.PropsObjectConfig config = GetPropsObjectConfigByType(model.HexType);
            PropsObject prefab = config.PropsObject;
            PropsObject instance = Object.Instantiate(prefab, root);
            instance.SetHex(model.Position);
            instance.transform.position = position; //_layout.ToPlane((Hex3d)hexModel);
            instance.gameObject.name = model.Position.ToString();

            if (model.Data.TryGetValue(PropsParamsNameConstants.Rotation, out string rotation))
            {
                var stepNum = int.Parse(rotation);
                instance.transform.Rotate(Vector3.up, MapConstants.AngleStep * stepNum);
            }
            
            return instance;
        }
        
        //#85925650 дублирование
        private PropsPrefabConfig.PropsObjectConfig GetPropsObjectConfigByType(string propsTypeName)
        {
            
            if (!_propsPrefabConfig.PropsObjectConfigs.TryGetValue(propsTypeName, out var propsObject))
            {
                throw new ArgumentException($"Unknown or undefined type {propsTypeName}");
            }
            
            return propsObject;
        }
    }
}