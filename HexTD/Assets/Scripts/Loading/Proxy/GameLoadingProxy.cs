using System;
using Cysharp.Threading.Tasks;

namespace Loading.Proxy
{
	// Used in PROJECT context to transition active scene GameLoadingService
	public class GameLoadingProxy : IGameLoadingService, IGameLoadingProxy
	{
		private IGameLoadingService gameLoadingService;

		UniTask<bool> IGameLoadingService.LoadGame(IProgress<float> loadingProgress) =>
			gameLoadingService?.LoadGame(loadingProgress) ?? UniTask.FromResult(false);

		void IGameLoadingProxy.ChangeLoadingService(IGameLoadingService loadingService) =>
			gameLoadingService = loadingService;
	}
}