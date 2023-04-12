using System;
using Cysharp.Threading.Tasks;
using UI.Loading_Window;
using WindowSystem;
using Zenject;

namespace Loading
{
	public class GameBootstrap : IInitializable
	{
		private readonly IGameLoadingService _gameLoadingService;
		private readonly IWindowsManager _windowsManager;

		public GameBootstrap(
			IGameLoadingService gameLoadingService,
			IWindowsManager windowsManager)
		{
			_gameLoadingService = gameLoadingService;
			_windowsManager = windowsManager;
		}

		public void Initialize() => InternalInitialize().Forget();

		private async UniTaskVoid InternalInitialize()
		{
			var loadingController = await _windowsManager.OpenAsync<LoadingWindowController>();
			loadingController.SetActiveLoading(true);
			loadingController.SetProgress(0);

			var loading = new LoadingCalculator(3, loadingController.SetProgress);

			loading.ChangeLoadingWeight(1);
			await _gameLoadingService.LoadGame(loading);

			await loadingController.WaitWhileProgressbarAnimationFinished();
			await loadingController.CloseWindowAsync();
		}

		private class LoadingCalculator : IProgress<float>
		{
			private readonly Action<float> _progress;
			private readonly float _sumWeights;

			private float _passedNormalizedWeights;
			private float _normalizedSumWeight;

			public LoadingCalculator(float sumWeights, Action<float> progress)
			{
				_progress = progress;
				_sumWeights = sumWeights;
			}

			public void ChangeLoadingWeight(float value)
			{
				_passedNormalizedWeights += _normalizedSumWeight;
				_normalizedSumWeight = value / _sumWeights;
			}

			void IProgress<float>.Report(float value)
			{
//				var progress01 = passedNormalizedWeights + normalizedSumWeight * value;
				_progress.Invoke(value);
			}
		}
	}
}