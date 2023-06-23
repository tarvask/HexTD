using Configs;
using Configs.Constants;
using MapEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Match.Field
{
	public class PropsObjectFabric
    {
        private readonly IPropsObjectPrefabConfigRetriever _propsPrefabConfigRetriever;

        public PropsObjectFabric(IPropsObjectPrefabConfigRetriever propsPrefabConfigRetriever)
        {
            _propsPrefabConfigRetriever = propsPrefabConfigRetriever;
        }

        public PropsObject Create(PropsModel model, Transform root, Vector3 position)
        {
            PropsPrefabConfig.PropsObjectConfig config = _propsPrefabConfigRetriever.GetPropsByType(model.HexType);
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
    }
}