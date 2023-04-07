using UnityEngine.AddressableAssets;

namespace WindowSystem.View.Factories.Addressable
{
	public readonly struct AssetReferenceInfo
	{
		public readonly AssetReference AssetReference;
		public readonly bool CacheInMemory;

		public AssetReferenceInfo(AssetReference assetReference, bool cacheInMemory)
		{
			CacheInMemory = cacheInMemory;
			AssetReference = assetReference;
		}
	}
}