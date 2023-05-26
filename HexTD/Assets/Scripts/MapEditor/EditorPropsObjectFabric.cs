using Match.Field;
using UnityEngine;

namespace MapEditor
{
	public class EditorPropsObjectFabric: PropsObjectFabric
	{
		private readonly Transform _rootTransform;
        
		public EditorPropsObjectFabric(IPropsObjectPrefabConfigRetriever propsPrefabConfigRetriever) : base(propsPrefabConfigRetriever)
		{
			// create cells root
			_rootTransform = new GameObject("Props").transform;
			_rootTransform.SetAsLastSibling();
			_rootTransform.localPosition = Vector3.zero;
			_rootTransform.localScale = Vector3.one;
		}

		public PropsObject Create(PropsModel model, Vector3 position)
		{
			return Create(model, _rootTransform, position);
		}
	}
}