using System.Collections.Generic;
using UnityEngine;

namespace Common.MemoryPool.Mono
{
    public class MonoMemoryPool<T> : IMemoryPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly int _maxSize;
        private readonly Transform _container;
        private readonly Queue<T> _pool;

        public MonoMemoryPool(
            T prefab,
            int maxSize,
            Transform container)
        {
            _prefab = prefab;
            _maxSize = maxSize;
            _container = container;
            _pool = new Queue<T>(maxSize);
        }

        private void CreateItem()
        {
            var item = Object.Instantiate(_prefab, _container, false);
            item.gameObject.SetActive(false);
            _pool.Enqueue(item);
        }

        public T Get()
        {
            if (_pool.Count == 0)
                CreateItem();

            return _pool.Dequeue();
        }

        public void Return(T obj)
        {
            if (_pool.Count < _maxSize)
            {
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(_container, false);
                _pool.Enqueue(obj);
            }
            else
            {
                Object.Destroy(obj.gameObject);
            }
        }
    }
}