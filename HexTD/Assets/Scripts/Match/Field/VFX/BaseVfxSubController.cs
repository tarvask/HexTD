using UnityEngine;

namespace Match.Field.VFX
{
    public abstract class BaseVfxSubController : MonoBehaviour
    {
        public abstract void Play();
        public abstract void Stop();
    }
}