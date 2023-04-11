using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Addressables
{
	public class AssetReferenceContainer<TKey> : IDisposable
	{
		private readonly Dictionary<TKey, AsyncOperationHandle<GameObject>> _assetsHandlersMap;

		public AssetReferenceContainer(int capacity) =>
			_assetsHandlersMap = new Dictionary<TKey, AsyncOperationHandle<GameObject>>(capacity);

		public AssetReferenceContainer() : this(0)
		{
		}

		private bool Contains(TKey key) =>
			_assetsHandlersMap.TryGetValue(key, out var handler) &&
			handler.IsValid();

		public void CacheInMemory(TKey key, AsyncOperationHandle<GameObject> assetHandler)
		{
			if (Contains(key))
				return;

			_assetsHandlersMap[key] = assetHandler;
		}

		public AsyncOperationHandle<GameObject> GetCached(TKey key) => _assetsHandlersMap[key];

		public void RemoveFromMemory(TKey key)
		{
			if (_assetsHandlersMap.TryGetValue(key, out var operationHandle))
			{
				_assetsHandlersMap.Remove(key);
				ReleaseOperationHandle(operationHandle);
			}
		}

		public void Dispose()
		{
			foreach (var assetHandler in _assetsHandlersMap.Values)
				ReleaseOperationHandle(assetHandler);

			_assetsHandlersMap.Clear();
		}

		private static void ReleaseOperationHandle(AsyncOperationHandle<GameObject> assetHandler)
		{
			if (assetHandler.IsValid() && assetHandler.Status == AsyncOperationStatus.Succeeded)
				UnityEngine.AddressableAssets.Addressables.Release(assetHandler);
		}
	}
}