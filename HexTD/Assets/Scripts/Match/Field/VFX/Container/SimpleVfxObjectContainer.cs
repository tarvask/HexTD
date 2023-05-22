using System.Collections.Generic;
using UnityEngine;

namespace Match.Field.VFX
{
    public class SimpleVfxObjectContainer : IVfxObjectContainer
    {
        private readonly Transform _containerTransform;
        private readonly List<ParticleSystem> _particleSystems;
        private readonly List<Animator> _animators;
        private readonly List<BaseVfxSubController> _vfxControllers;

        public IEnumerable<ParticleSystem> ParticleSystems => _particleSystems;
        public IEnumerable<Animator> Animators => _animators;
        public IEnumerable<BaseVfxSubController> VfxControllers => _vfxControllers;

        public SimpleVfxObjectContainer(Transform transform)
        {
            _containerTransform = transform;
            _particleSystems = new List<ParticleSystem>();
            _animators = new List<Animator>();
            _vfxControllers = new List<BaseVfxSubController>();

            AddAllVfxObjects(transform);
        }

        public void SetPosition(Vector3 position)
        {
            _containerTransform.position = position;
        }

        public IVfxObjectContainer CloneContainer(Transform parent)
        {
            Transform transform = Object.Instantiate(_containerTransform, parent);
            return new SimpleVfxObjectContainer(transform); 
        }

        private void AddAllVfxObjects(Transform parent)
        {
            GameObject parentObject = parent.gameObject;
            if (!parentObject.activeSelf)
                return;
            
            if(parent.TryGetComponent<ParticleSystem>(out var vfxParticleSystem))
                _particleSystems.Add(vfxParticleSystem);
            if(parent.TryGetComponent<Animator>(out var animator))
                _animators.Add(animator);
            if (parent.TryGetComponent<BaseVfxSubController>(out var vfxController))
                _vfxControllers.Add(vfxController);
            
            foreach (Transform childObject in parent)
            {
                AddAllVfxObjects(childObject);
            }
        }

        public void Dispose()
        {
            _particleSystems.Clear();
            _animators.Clear();
            _vfxControllers.Clear();
            
            Object.Destroy(_containerTransform.gameObject);
        }
    }
}