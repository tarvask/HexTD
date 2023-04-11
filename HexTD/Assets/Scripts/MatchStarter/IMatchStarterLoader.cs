using System;
using Cysharp.Threading.Tasks;
using UniRx;

namespace MatchStarter
{
	public interface IMatchStarterLoader
	{
		bool IsLoading { get; }

		IObservable<Unit> LocationLoaded { get; }

		UniTask<MatchStarter> LoadAsync(bool autoComplete = true);

		void DestroyAndRelease();
	}
}