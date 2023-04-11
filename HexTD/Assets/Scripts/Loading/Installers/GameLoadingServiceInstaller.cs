using UnityEngine;
using Zenject;

namespace Loading.Installers
{
	public class GameLoadingServiceInstaller : MonoInstaller
	{
		[SerializeField] private GameLoadingStep[] loadingSteps;

		public override void InstallBindings()
		{
			foreach (var step in loadingSteps)
				Container.QueueForInject(step);
			
			var loadingService = Container.Instantiate<GameLoadingService>(new[] { loadingSteps });

			Container.Rebind<IGameLoadingService>()
				.To<GameLoadingService>()
				.FromInstance(loadingService)
				.AsTransient()
				.WithArguments(loadingSteps);
		}
	}
}