using System;
using System.Collections.Generic;
using Tools.Interfaces;
using UnityEngine;

namespace Match.Field.VFX
{
    public abstract class BaseVfxObjectPool<TKey> : IOuterLogicUpdatable, IDisposable
    {
        private readonly Transform _parentTransform;
        private readonly IVfxObjectContainer _prefabVfxObject;

        private readonly Dictionary<TKey, VfxController> _activeVfxControllers;
        private readonly Stack<VfxController> _pooledVfxControllers;

        public IReadOnlyDictionary<TKey, VfxController> ActiveVfxControllers => _activeVfxControllers;

        public BaseVfxObjectPool(Transform vfxObjectTransformPrefab, Transform parentTransform = null)
        {
            if (parentTransform == null)
                parentTransform = new GameObject($"{vfxObjectTransformPrefab.name}s").transform;
            
            _parentTransform = parentTransform;

            if (!vfxObjectTransformPrefab.TryGetComponent<IVfxObjectContainer>(out _prefabVfxObject))
                _prefabVfxObject = new SimpleVfxObjectContainer(vfxObjectTransformPrefab);
            
            _activeVfxControllers = new Dictionary<TKey, VfxController>();
            _pooledVfxControllers = new Stack<VfxController>(3);
        }

        public void InitFreeController(TKey key)
        {
            if(_activeVfxControllers.ContainsKey(key))
                return;
            
            if (_pooledVfxControllers.Count == 0)
                PooledNewVfxObject();

            VfxController vfxController = _pooledVfxControllers.Pop();
            _activeVfxControllers.Add(key, vfxController);
            vfxController.Play();
        }

        public void ReleaseVfx(TKey key)
        {
            if(!_activeVfxControllers.Remove(key, out var vfxController))
                return;
            
            vfxController.Stop();
            _pooledVfxControllers.Push(vfxController);
        }

        private void PooledNewVfxObject()
        {
            IVfxObjectContainer vfxObjectContainer = _prefabVfxObject.CloneContainer(_parentTransform);
            VfxController vfxController = new VfxController(vfxObjectContainer);
            _pooledVfxControllers.Push(vfxController);
        }

        public abstract void OuterLogicUpdate(float frameLength);
        
        public void Dispose()
        {
            foreach (var activeVfxController in _activeVfxControllers.Values)
                activeVfxController.Dispose();

            foreach (var pooledVfxController in _pooledVfxControllers)
                pooledVfxController.Dispose();
        }
    }
}