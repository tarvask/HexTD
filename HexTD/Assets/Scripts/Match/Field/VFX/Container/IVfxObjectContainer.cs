using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match.Field.VFX
{
    public interface IVfxObjectContainer : IDisposable
    {
        public Transform Transform { get; }
        public IEnumerable<ParticleSystem> ParticleSystems { get; }
        public IEnumerable<Animator> Animators { get; }
        public IEnumerable<BaseVfxSubController> VfxControllers { get; }

        public void SetPosition(Vector3 position);
        public IVfxObjectContainer CloneContainer(Transform parent);
    }
}