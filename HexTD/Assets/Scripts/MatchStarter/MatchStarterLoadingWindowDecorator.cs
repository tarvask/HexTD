using System;
using Cysharp.Threading.Tasks;
using UI.Loading_Window;
using UniRx;
using WindowSystem;

namespace MatchStarter
{
	public class MatchStarterLoadingWindowDecorator : IMatchStarterLoader, IDisposable
	{
		private readonly IMatchStarterLoader _locationLoader;
		private readonly IWindowsManager _windowsManager;
		private readonly Subject<Unit> _locationLoaded = new Subject<Unit>();
		private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

		private bool _isLoading;

		public MatchStarterLoadingWindowDecorator(
			IMatchStarterLoader locationLoader,
			IWindowsManager windowsManager)
		{
			_locationLoader = locationLoader;
			_windowsManager = windowsManager;

			locationLoader.LocationLoaded
				.Subscribe(value => _locationLoaded.OnNext(value))
				.AddTo(_compositeDisposable);
		}

		public bool IsLoading => _locationLoader.IsLoading || _isLoading;

		public IObservable<Unit> LocationLoaded => _locationLoaded;

		public async UniTask<MatchStarter> LoadAsync(bool autoComplete = true)
		{
			_isLoading = true;
			var loadingController = await _windowsManager.OpenSingleAsync<LoadingWindowController>();
			loadingController.SetActiveLoading(true);
			loadingController.SetProgress(0.5f);

			var matchStarter = await _locationLoader.LoadAsync();

			loadingController.SetProgress(1f);

			if (autoComplete)
				await CompleteLoading(loadingController);

			return matchStarter;
		}

		public void DestroyAndRelease()
		{
			_locationLoader.DestroyAndRelease();
		}

		private async UniTask CompleteLoading(LoadingWindowController loadingController)
		{
			await loadingController.WaitWhileProgressbarAnimationFinished();

			_locationLoaded.OnNext(Unit.Default);

			await loadingController.CloseWindowAsync();

			_isLoading = false;
		}

		void IDisposable.Dispose()
		{
			_compositeDisposable.Dispose();
		}
	}
}