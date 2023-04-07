using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Tools.Addressables;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Common.MemoryPool.Addressable
{
    public class AddressableMemoryPool<T> : IDisposable, IAsyncMemoryPool<T> where T : Component
    {
        private readonly Transform _container;
        private readonly AddressablePoolSettings _settings;
        private readonly Queue<T> _pool;
        private readonly AssetReferenceContainer<string> _assetsReferenceCache;

        private int _lastAllocationFrame;
        private int _allocatedComponentsCount;

        public AddressableMemoryPool(AddressablePoolSettings settings)
        {
            _settings = settings;
            _pool = new Queue<T>(settings.maxSize);
            _container = new GameObject(settings.poolName).transform;
            _assetsReferenceCache = new AssetReferenceContainer<string>(settings.maxSize);
        }

        public async UniTask<T> GetAsync()
        {
            if (_pool.Count == 0)
                await CreateItemAsync();

            return _pool.Dequeue();
        }

        private async UniTask CreateItemAsync()
        {
            var instanceHandle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(_settings.reference);
            await instanceHandle;
            _assetsReferenceCache.CacheInMemory(_settings.reference.AssetGUID, instanceHandle);
            var componentPrefab = instanceHandle.Result.GetComponent<T>();
            var activeFrame = Time.frameCount;

            if (activeFrame == _lastAllocationFrame &&
                _allocatedComponentsCount >= _settings.perFrameAllocation)
            {
                await UniTask.WaitForEndOfFrame();
                _allocatedComponentsCount = 0;
            }

            var item = Object.Instantiate(componentPrefab, _container, false);
            _allocatedComponentsCount++;
            _lastAllocationFrame = activeFrame;
            item.gameObject.SetActive(false);
            _pool.Enqueue(item);
        }

        public void Return(T instance)
        {
            if (_pool.Count < _settings.maxSize)
            {
                instance.gameObject.SetActive(false);
                instance.transform.SetParent(_container, false);
                _pool.Enqueue(instance);
            }
            else
                Object.Destroy(instance.gameObject);
        }

        public void Dispose() => _assetsReferenceCache.Dispose();
    }
}