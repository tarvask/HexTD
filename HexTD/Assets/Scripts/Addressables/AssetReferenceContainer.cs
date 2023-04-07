using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Addressables
{
	public class AssetReferenceContainer<TKey> : IDisposable
	{
		private readonly Dictionary<TKey, AsyncOperationHandle<GameObject>> assetsHandlersMap;

		public AssetReferenceContainer(int capacity) =>
			assetsHandlersMap = new Dictionary<TKey, AsyncOperationHandle<GameObject>>(capacity);

		public AssetReferenceContainer() : this(0)
		{
		}

		private bool Contains(TKey key) =>
			assetsHandlersMap.TryGetValue(key, out var handler) &&
			handler.IsValid();

		public void CacheInMemory(TKey key, AsyncOperationHandle<GameObject> assetHandler)
		{
			if (Contains(key))
				return;

			assetsHandlersMap[key] = assetHandler;
		}

		public AsyncOperationHandle<GameObject> GetCached(TKey key) => assetsHandlersMap[key];

		public void RemoveFromMemory(TKey key)
		{
			if (assetsHandlersMap.TryGetValue(key, out var operationHandle))
			{
				assetsHandlersMap.Remove(key);
				ReleaseOperationHandle(operationHandle);
			}
		}

		public void Dispose()
		{
			foreach (var assetHandler in assetsHandlersMap.Values)
				ReleaseOperationHandle(assetHandler);

			assetsHandlersMap.Clear();
		}

		private static void ReleaseOperationHandle(AsyncOperationHandle<GameObject> assetHandler)
		{
			if (assetHandler.IsValid() && assetHandler.Status == AsyncOperationStatus.Succeeded)
				UnityEngine.AddressableAssets.Addressables.Release(assetHandler);
		}
	}
}