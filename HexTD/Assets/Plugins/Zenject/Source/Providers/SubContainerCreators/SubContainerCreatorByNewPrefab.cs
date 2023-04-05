#if !NOT_UNITY3D

using System.Collections.Generic;
using Plugins.Zenject.Source.Binding.BindInfo;
using Plugins.Zenject.Source.Injection;
using Plugins.Zenject.Source.Install.Contexts;
using Plugins.Zenject.Source.Internal;
using Plugins.Zenject.Source.Main;
using Plugins.Zenject.Source.Providers.PrefabProviders;
using Zenject;

namespace Plugins.Zenject.Source.Providers.SubContainerCreators
{
    [NoReflectionBaking]
    public class SubContainerCreatorByNewPrefab : ISubContainerCreator
    {
        readonly GameObjectCreationParameters _gameObjectBindInfo;
        readonly IPrefabProvider _prefabProvider;
        readonly DiContainer _container;

        public SubContainerCreatorByNewPrefab(
            DiContainer container, IPrefabProvider prefabProvider,
            GameObjectCreationParameters gameObjectBindInfo)
        {
            _gameObjectBindInfo = gameObjectBindInfo;
            _prefabProvider = prefabProvider;
            _container = container;
        }

        public DiContainer CreateSubContainer(List<TypeValuePair> args, InjectContext parentContext)
        {
            Assert.That(args.IsEmpty());

            var prefab = _prefabProvider.GetPrefab();
            var gameObject = _container.InstantiatePrefab(prefab, _gameObjectBindInfo);

            var context = gameObject.GetComponent<GameObjectContext>();

            Assert.That(context != null,
                "Expected prefab with name '{0}' to container a component of type 'GameObjectContext'", prefab.name);

            // Note: We don't need to call ResolveRoots here because GameObjectContext does this for us

            return context.Container;
        }
    }
}

#endif
