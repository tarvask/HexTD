#if !NOT_UNITY3D

namespace Plugins.Zenject.Source.Providers.PrefabProviders
{
    public interface IPrefabProvider
    {
        UnityEngine.Object GetPrefab();
    }
}

#endif

