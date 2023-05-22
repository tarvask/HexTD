using System;
using UnityEngine;

namespace Match.Field.VFX
{
    public class VfxController : IDisposable
    {
        private static readonly int IsVfx = Animator.StringToHash("isVfx");
        
        private readonly IVfxObjectContainer _vfxObjectContainer;

        public VfxController(IVfxObjectContainer vfxObjectContainer)
        {
            _vfxObjectContainer = vfxObjectContainer;
        }

        public void Play()
        {
            foreach (var particleSystem in _vfxObjectContainer.ParticleSystems)
            {
                particleSystem.Play();
                particleSystem.gameObject.SetActive(true);
            }

            foreach (var animator in _vfxObjectContainer.Animators)
            {
                animator.SetBool(IsVfx, true);
                animator.gameObject.SetActive(true);                
            }

            foreach (var vfxController in _vfxObjectContainer.VfxControllers)
            {
                vfxController.Play();
                vfxController.gameObject.SetActive(true);
            }
            
            _vfxObjectContainer.Transform.gameObject.SetActive(true);
        }

        public void Stop()
        {
            foreach (var particleSystem in _vfxObjectContainer.ParticleSystems)
            {
                particleSystem.Stop();
                particleSystem.gameObject.SetActive(false);
            }

            foreach (var animator in _vfxObjectContainer.Animators)
            {
                animator.SetBool(IsVfx, false);
                animator.gameObject.SetActive(false);                
            }

            foreach (var vfxController in _vfxObjectContainer.VfxControllers)
            {
                vfxController.Stop();
                vfxController.gameObject.SetActive(false);
            }
            
            _vfxObjectContainer.Transform.gameObject.SetActive(false);
        }

        public void SetPosition(Vector3 position)
        {
            _vfxObjectContainer.SetPosition(position);
        }
        
        public void Dispose()
        {
            _vfxObjectContainer.Dispose();
        }
    }
}