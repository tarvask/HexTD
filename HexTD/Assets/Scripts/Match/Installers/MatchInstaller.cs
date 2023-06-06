using Match.Field;
using Match.Field.Services;
using Match.Field.Tower;
using Match.Field.VFX;
using UI.ScreenSpaceOverlaySystem;
using UnityEngine;
using Zenject;

namespace Match.Installers
{
	public class MatchInstaller: MonoInstaller
	{
		[Inject]
		MatchController.Context _matchControllerContext;
		
		[SerializeField] private MatchView matchView;
		[SerializeField] private Canvas canvas;
		[SerializeField] private Camera ourFieldCamera;
		[SerializeField] private GameObject screenSpaceOverlayPrefab;
		
		public override void InstallBindings()
		{
			Container.BindInstance(matchView).AsSingle();
			
			Container.BindInstance(canvas).AsSingle();
			
			Container.BindInstance(ourFieldCamera)
				.WithId("OurFieldCamera")
				.AsSingle();
			
			Container.BindInstance(_matchControllerContext).WhenInjectedInto<MatchController>();
			Container.Bind<MatchController>().FromNew().AsSingle();
			Container.BindFactory<FieldController.Context, FieldController, FieldController.Factory>().AsSingle();
			Container.BindFactory<FieldFactory.Context, FieldFactory, FieldFactory.Factory>().AsSingle();
			
			Container.BindFactory<VfxManager, int, TowersManager, TowersManager.Factory>().AsSingle();
			Container.BindFactory<MobsManager.Context, MobsManager, MobsManager.Factory>().AsSingle();
			
			Container.Bind<ScreenSpaceOverlayController>()
				.FromSubContainerResolve()
				.ByNewContextPrefab(screenSpaceOverlayPrefab)
				.UnderTransform(canvas.transform)
				.AsSingle()
				.NonLazy();
		}
	}
}