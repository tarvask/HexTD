using System.Collections.Generic;
using UnityEngine;

namespace Match.Field.VFX
{
    public class VfxObjectContainerComponent : MonoBehaviour, IVfxObjectContainer
    {
        [SerializeField] private List<ParticleSystem> particleSystems;
        [SerializeField] private List<Animator> animators;
        [SerializeField] private List<BaseVfxSubController> vfxControllers;

        public Transform Transform => transform;
        public IEnumerable<ParticleSystem> ParticleSystems => particleSystems;
        public IEnumerable<Animator> Animators => animators;
        public IEnumerable<BaseVfxSubController> VfxControllers => vfxControllers;
        
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public IVfxObjectContainer CloneContainer(Transform parent)
        {
            return Instantiate(this, parent);
        }
        
        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}