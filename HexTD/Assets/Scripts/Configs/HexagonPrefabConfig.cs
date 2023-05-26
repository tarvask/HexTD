using System;
using System.Collections.Generic;
using MapEditor;
using Tools;
using UnityEngine;

namespace Configs
{
	[CreateAssetMenu(fileName = "HexagonPrefabConfig", menuName = "Configs/HexagonPrefabConfig", order = 4)]
	public class HexagonPrefabConfig : ScriptableObject
	{
		[SerializeField] private StringHexObjectDictionary hexObjects;
		public IReadOnlyDictionary<string, HexObject> HexObjects => hexObjects;

		[Serializable]
		public class StringHexObjectDictionary : UnitySerializedDictionary<string, HexObject>
		{
		}
	}
}