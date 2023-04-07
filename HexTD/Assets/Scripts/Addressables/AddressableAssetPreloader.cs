using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Addressables
{
	public class AddressableAssetPreloader
	{
		private readonly List<AssetReference> references = new List<AssetReference>();

		public bool AssetsPreloaded { get; private set; } = false;

		public void RegisterReference(AssetReference assetReference)
		{
			references.Add(assetReference);
		}

		public async UniTask PreloadAssetsAsync()
		{
			AssetsPreloaded = false;
			foreach (var reference in references)
			{
				await reference.LoadAssetAsync<GameObject>();
			}

			AssetsPreloaded = true;
		}
	}
}