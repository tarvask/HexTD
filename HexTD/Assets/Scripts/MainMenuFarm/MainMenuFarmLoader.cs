using System;
using System.Threading.Tasks;
using Addressables;
using Cysharp.Threading.Tasks;
using FarmDataBase;
using UniRx;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using Object = UnityEngine.Object;

namespace MainMenuFarm
{
    public class MainMenuFarmLoader : IMainMenuFarmLoader
    {
        public bool IsLoading { get; private set; }

        public IObservable<Unit> LocationLoaded => _locationLoaded;

        private readonly AssetsDownloadingProvider _downloadingProvider;
        private readonly FarmSettings _farmSettings;
        private readonly IInstantiator _instantiator;
        private readonly Subject<Unit> _locationLoaded = new Subject<Unit>();

        private AsyncOperationHandle<GameObject>? _locationHandle;
        private GameObject _currentLocation;

        public MainMenuFarmLoader(
            AssetsDownloadingProvider downloadingProvider,
            IInstantiator instantiator,
            Subject<Unit> locationLoaded,
            FarmSettings farmSettings)
        {
            _downloadingProvider = downloadingProvider;
            _instantiator = instantiator;
            _locationLoaded = locationLoaded;
            _farmSettings = farmSettings;
        }

        public async UniTask<MainMenuFarm> LoadAsync(bool autoComplete = true)
        {
            IsLoading = true;

            DestroyAndRelease();

            await _downloadingProvider.DownloadAssetWithLabelIfRequiredAsync(_farmSettings.FarmLabel);
            await InitLocationHandle();

            if (_locationHandle.HasValue)
                _currentLocation = _instantiator.InstantiatePrefab(_locationHandle.Value.Result);

            var farm = _currentLocation.transform.GetComponent<MainMenuFarm>();

            if (farm)
                await UniTask.WaitUntil(() => farm.IsLoaded);

            if (autoComplete)
            {
                _locationLoaded.OnNext(Unit.Default);
                IsLoading = false;
            }

            return farm;
        }

        public UniTask CompleteAsync()
        {
            if (IsLoading)
            {
                _locationLoaded.OnNext(Unit.Default);
                IsLoading = false;
            }

            return UniTask.CompletedTask;
        }

        private async Task InitLocationHandle()
        {
            if (_locationHandle.HasValue)
            {
                return;
            }

            _locationHandle = _farmSettings.FarmPrefabReference.LoadAssetAsync();

            await _locationHandle.Value;
        }

        public void DestroyAndRelease()
        {
            if (_currentLocation != null)
            {
                Object.Destroy(_currentLocation);
            }

            if (_locationHandle.HasValue) //&& _locationHandle.Value.IsDone && _locationHandle.Value.IsValid())
                UnityEngine.AddressableAssets.Addressables.Release(_locationHandle.Value);

            _locationHandle = null;
        }
    }
}