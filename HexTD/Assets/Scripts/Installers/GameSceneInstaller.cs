using System;
using Addressables;
using MainMenuFarm;
using MatchStarter;
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
            
            Container.Bind<AssetsDownloadingProvider>().AsSingle();

            BindMatchStarterLoader();
            BindMainMenuFarmLoader();

            Container.Bind<EventSystem>().FromComponentInNewPrefab(eventSystem).AsSingle().NonLazy();
        }

//        private void BindLocationLoader()
//        {
//            Container.Bind(typeof(ILocationLoader), typeof(LocationLoader))
//                .To<LocationLoader>().AsSingle();
//            Container.Decorate<ILocationLoader>().With<LocationLoadingWindowDecorator>();
//        }

        private void BindMatchStarterLoader()
        {
            Container.Bind(typeof(IMatchStarterLoader))
                .To<MatchStarterLoader>().AsSingle();
            Container.Decorate<IMatchStarterLoader>().With<MatchStarterLoadingWindowDecorator>();
        }

        private void BindMainMenuFarmLoader()
        {
            Container.Bind(typeof(IMainMenuFarmLoader))
                .To<MainMenuFarmLoader>().AsSingle();
        }

        //        private void BindFactoryProductInstance<TProduct, TFactory>() where TFactory : PlaceholderFactory<TProduct>
        //        {
        //            Container.Bind<TProduct>()
        //                .FromMethod(() => Container.Resolve<TFactory>().Create())
        //                .AsSingle();
        //        }
    }
}