using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using SceneManagement.Flow;
using UI.Loading_Window;
using UnityEngine;
using WindowSystem;
using Zenject;
using Object = UnityEngine.Object;

namespace Loading
{
	public class GameBootstrap : IInitializable
	{
		private readonly SceneFlow[] sceneFlows;
		private readonly GameObject splashScreen;
		private readonly IGameLoadingService gameLoadingService;
		private readonly IWindowsManager windowsManager;

		public GameBootstrap(
			SceneFlow[] sceneFlows,
			GameObject splashScreen,
			IGameLoadingService gameLoadingService,
			IWindowsManager windowsManager)
		{
			this.sceneFlows = sceneFlows;
			this.splashScreen = splashScreen;
			this.gameLoadingService = gameLoadingService;
			this.windowsManager = windowsManager;
		}

		public void Initialize() => InternalInitialize().Forget();

		private async UniTaskVoid InternalInitialize()
		{
			var loadingController = await windowsManager.OpenAsync<LoadingWindowController>();
			loadingController.SetActiveLoading(true);
			loadingController.SetProgress(0);
			Object.Destroy(splashScreen);
			var loading = new LoadingCalculator(sceneFlows.Sum(x => x.Weight), loadingController.SetProgress);

			for (var i = 0; i < sceneFlows.Length; i++)
			{
				var flow = sceneFlows[i];
				loading.ChangeLoadingWeight(flow.Weight);
				await flow.RunSceneFlow();
				await gameLoadingService.LoadGame(loading);
				if (flow.TransitionScene)
					await flow.StopSceneFlow();
			}

			await windowsManager.CloseAsync(loadingController);
		}

		private class LoadingCalculator : IProgress<float>
		{
			private readonly Action<float> progress;
			private readonly float sumWeights;

			private float passedNormalizedWeights;
			private float normalizedSumWeight;

			public LoadingCalculator(float sumWeights, Action<float> progress)
			{
				this.progress = progress;
				this.sumWeights = sumWeights;
			}

			public void ChangeLoadingWeight(float value)
			{
				passedNormalizedWeights += normalizedSumWeight;
				normalizedSumWeight = value / sumWeights;
			}

			void IProgress<float>.Report(float value)
			{
				var progress01 = passedNormalizedWeights + normalizedSumWeight * value;
				progress.Invoke(progress01);
			}
		}
	}
}