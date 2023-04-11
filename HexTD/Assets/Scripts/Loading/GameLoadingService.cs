using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Loading
{
	public class GameLoadingService : IGameLoadingService
	{
		private readonly IList<GameLoadingStep> _steps;
		private readonly float _loadingWeight = 0;

		public GameLoadingService(IList<GameLoadingStep> steps)
		{
			_steps = steps;
			_loadingWeight = steps.Sum(x => x.StepWeight);
		}

		public async UniTask<bool> LoadGame(IProgress<float> loadingProgress)
		{
			Log("Start game loading..");
			var currentLoading = 0;

			foreach (var step in _steps)
			{
				var stopwatch = Stopwatch.StartNew();
				await step.LoadStep();
				Log($"{step.GetType().Name} successfully loaded");
				currentLoading += step.StepWeight;
				var progressValue = currentLoading / _loadingWeight;
				loadingProgress.Report(progressValue);
				Log($"Game loading progress : {progressValue.ToString(CultureInfo.CurrentCulture)}");
				stopwatch.Stop();
				Log($"Step loading time : {stopwatch.ElapsedMilliseconds}");
			}

			return true;
		}

		private static void Log(string message) => UnityEngine.Debug.Log($"<b><color=green>{message}</color></b>");
	}
}