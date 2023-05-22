using System;
using System.Collections.Generic;
using Match.Field.Shooting;
using Tools.Interfaces;
using UnityEngine;

namespace Match.Field.VFX
{
    public class VfxManager : IOuterLogicUpdatable, IDisposable
    {
        private const int VfxPoolCapacity = 5;

        private readonly Transform _parentTransform;
        private readonly Dictionary<Transform, TargetVfxObjectPool> _vfxObjectPools;

        public VfxManager()
        {
            _parentTransform = new GameObject("VfxManager").transform;
            _vfxObjectPools = new Dictionary<Transform, TargetVfxObjectPool>(VfxPoolCapacity);
        }

        public void AddVfx(ITarget target, Transform vfxPrefab)
        {
            if (!_vfxObjectPools.TryGetValue(vfxPrefab, out var targetVfxObjectPool))
            {
                Transform parentTransform = new GameObject($"{vfxPrefab.name}s").transform;
                parentTransform.parent = _parentTransform;
                targetVfxObjectPool = new TargetVfxObjectPool(vfxPrefab, parentTransform);
                _vfxObjectPools.Add(vfxPrefab, targetVfxObjectPool);
            }

            targetVfxObjectPool.InitFreeController(target);
        }

        public void ReleaseVfx(ITarget target)
        {
            foreach (var targetVfxPool in _vfxObjectPools.Values)
            {
                targetVfxPool.ReleaseVfx(target);
            }
        }

        public void OuterLogicUpdate(float frameLength)
        {
            foreach (var targetVfxObjectPool in _vfxObjectPools)
            {
                targetVfxObjectPool.Value.OuterLogicUpdate(frameLength);
            }
        }
        
        public void Dispose()
        {
            foreach (var targetVfxPool in _vfxObjectPools.Values)
            {
                targetVfxPool.Dispose();
            }
        }
    }
}