using System;
using Cysharp.Threading.Tasks;
using UI.Loading_Window;
using UniRx;
using UnityEngine;
using WindowSystem;

namespace Locations.Loading
{
    public class LocationLoadingWindowDecorator : ILocationLoader, IDisposable
    {
        private readonly ILocationLoader _locationLoader;
        private readonly IWindowsManager _windowsManager;
        private readonly Subject<string> _locationLoaded = new Subject<string>();
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private bool _isLoading;

        public LocationLoadingWindowDecorator(
            ILocationLoader locationLoader,
            IWindowsManager windowsManager)
        {
            _locationLoader = locationLoader;
            _windowsManager = windowsManager;

            locationLoader.LocationLoaded
                .Subscribe(locationId => _locationLoaded.OnNext(locationId))
                .AddTo(_compositeDisposable);
        }

        public bool IsLoading => _locationLoader.IsLoading || _isLoading;
        public GameObject CurrentLocation => _locationLoader.CurrentLocation;
        public string CurrentLocationId => _locationLoader.CurrentLocationId;
        public IObservable<string> LocationLoaded => _locationLoaded;

        public async UniTask LoadAsync(string locationId, bool autoComplete = true)
        {
            _isLoading = true;
            var loadingController = await _windowsManager.OpenSingleAsync<LoadingWindowController>();
            loadingController.SetActiveLoading(true);
            loadingController.SetProgress(0.5f);
            await _locationLoader.LoadAsync(locationId);
            loadingController.SetProgress(1f);

            if (autoComplete)
                await CompleteLoading(loadingController);
        }

        public async UniTask CompleteAsync()
        {
            await _locationLoader.CompleteAsync();
            var loadingController = _windowsManager.GetOpened<LoadingWindowController>();
            await CompleteLoading(loadingController);
        }

        public void HandleCityLocationLoading(string locationId) =>
            _locationLoader.HandleCityLocationLoading(locationId);

        private async UniTask CompleteLoading(LoadingWindowController loadingController)
        {
            await UniTask.WaitWhile(() => loadingController.InProgress);
            _locationLoaded.OnNext(CurrentLocationId);
            await _windowsManager.CloseAsync(loadingController);
            _isLoading = false;
        }

        void IDisposable.Dispose()
        {
            _compositeDisposable.Dispose();
        }
    }
}