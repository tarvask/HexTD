#if !NOT_UNITY3D

using Plugins.Zenject.Source.Internal;
using Zenject;

namespace Plugins.Zenject.Source.Providers.PrefabProviders
{
    [NoReflectionBaking]
    public class PrefabProvider : IPrefabProvider
    {
        readonly UnityEngine.Object _prefab;

        public PrefabProvider(UnityEngine.Object prefab)
        {
            Assert.IsNotNull(prefab);
            _prefab = prefab;
        }

        public UnityEngine.Object GetPrefab()
        {
            return _prefab;
        }
    }
}

#endif


