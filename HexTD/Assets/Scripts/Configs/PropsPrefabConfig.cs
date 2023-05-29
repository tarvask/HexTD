using System;
using System.Collections.Generic;
using MapEditor;
using Tools;
using UnityEngine;

namespace Configs
{
	[CreateAssetMenu(fileName = "PropsPrefabConfig", menuName = "Configs/PropsPrefabConfig", order = 4)]
	public class PropsPrefabConfig : ScriptableObject
	{
		[SerializeField] private StringAndPropsObjectConfigDictionary propsObjectConfigs;
		public IReadOnlyDictionary<string, PropsObjectConfig> PropsObjectConfigs => propsObjectConfigs;

		[Serializable]
		public class StringAndPropsObjectConfigDictionary : UnitySerializedDictionary<string, PropsObjectConfig>
		{
		}

		[Serializable]
		public class PropsObjectConfig
		{
			[SerializeField] private PropsObject propsObject;
			[SerializeField] private PropsPlacingConfig propsPlacingConfig;
			
			public PropsObject PropsObject => propsObject;
			public PropsPlacingConfig PropsPlacingConfig => propsPlacingConfig;
		}
	}
}