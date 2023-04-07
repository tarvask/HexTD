using System;
using Locations.Loading;
using Tools.Disposing;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private EventSystem eventSystem;
        
        public override void InstallBindings()
        {
            Container.Bind(typeof(IDisposable), typeof(IDisposableContext))
                .To<DisposableContext>().AsSingle();

            BindLocationLoader();
            
            Container.Bind<EventSystem>().FromComponentInNewPrefab(eventSystem).AsSingle().NonLazy();
        }

        private void BindLocationLoader()
        {
            Container.Bind(typeof(ILocationLoader), typeof(LocationLoader))
                .To<LocationLoader>().AsSingle();
            Container.Decorate<ILocationLoader>().With<LocationLoadingWindowDecorator>();
        }

//        private void BindFactoryProductInstance<TProduct, TFactory>() where TFactory : PlaceholderFactory<TProduct>
//        {
//            Container.Bind<TProduct>()
//                .FromMethod(() => Container.Resolve<TFactory>().Create())
//                .AsSingle();
//        }
    }
}