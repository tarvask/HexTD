using System;
using Cysharp.Threading.Tasks;

namespace Loading
{
	public interface IGameLoadingService
	{
		UniTask<bool> LoadGame(IProgress<float> loadingProgress);
	}
}