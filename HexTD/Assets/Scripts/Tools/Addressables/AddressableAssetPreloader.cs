using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Tools.Addressables
{
	public class AddressableAssetPreloader
	{
		private readonly List<AssetReference> _references = new List<AssetReference>();

		public bool AssetsPreloaded { get; private set; } = false;

		public void RegisterReference(AssetReference assetReference)
		{
			_references.Add(assetReference);
		}

		public async UniTask PreloadAssetsAsync()
		{
			AssetsPreloaded = false;
			foreach (var reference in _references)
			{
				await reference.LoadAssetAsync<GameObject>();
			}

			AssetsPreloaded = true;
		}
	}
}