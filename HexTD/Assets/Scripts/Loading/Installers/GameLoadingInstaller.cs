using Loading.Proxy;
using UnityEngine;
using Zenject;

namespace Loading.Installers
{
	// Scene context
	public class GameLoadingInstaller : MonoInstaller
	{
		[SerializeField] private GameLoadingStep[] loadingSteps;

		public override void InstallBindings()
		{
			foreach (var step in loadingSteps)
				Container.QueueForInject(step);


			var proxy = Container.Resolve<IGameLoadingProxy>();
			var loadingService = Container.Instantiate<GameLoadingService>(new[] { loadingSteps });
			proxy.ChangeLoadingService(loadingService);

			Container.Rebind<IGameLoadingService>()
				.To<GameLoadingService>()
				.FromInstance(loadingService)
				.AsTransient()
				.WithArguments(loadingSteps);
		}
	}
}