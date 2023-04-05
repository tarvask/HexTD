using SceneManagement.Flow;
using UnityEngine;
using Zenject;

namespace Loading.Installers
{
	// Entry scene context
	public class GameBootstrapInstaller : MonoInstaller
	{
		[SerializeField] private SceneFlow[] loadingFlows;
		[SerializeField] private GameObject splashScreen;

		public override void InstallBindings()
		{
#if DEVELOPMENT_BUILD
            DumpHelper.Initialize();
#endif
			foreach (var flow in loadingFlows)
				Container.QueueForInject(flow);

			Container.BindInterfacesTo<GameBootstrap>().AsSingle().WithArguments(loadingFlows, splashScreen);
		}
	}
}