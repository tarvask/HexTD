using System;
using UnityEngine.AddressableAssets;

namespace WindowSystem.View.Factories.Addressable
{
	[Serializable]
	public class WindowViewAsset
	{
#if UNITY_EDITOR
		public WindowViewBase view;
#endif
		public string viewType;
		public bool cacheInMemory;
		public AssetReference assetReference;
	}
}