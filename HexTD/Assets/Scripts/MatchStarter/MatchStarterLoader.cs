using System;
using System.Threading.Tasks;
using Addressables;
using Cysharp.Threading.Tasks;
using MatchDataBase;
using UniRx;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using Object = UnityEngine.Object;

namespace MatchStarter
{
	public class MatchStarterLoader : IMatchStarterLoader
	{
		public IObservable<Unit> LocationLoaded => _locationLoaded;
		public bool IsLoading { get; private set; }
		
		private readonly AssetsDownloadingProvider _downloadingProvider;
		private readonly MatchStarterSettings _matchStarterSettings;
		private readonly IInstantiator _instantiator;
		private readonly Subject<Unit> _locationLoaded = new Subject<Unit>();

//		private AsyncOperationHandle<GameObject>? _handleCache;

		private AsyncOperationHandle<GameObject>? _locationHandle;
		private GameObject _currentLocation;

		public MatchStarterLoader(
            AssetsDownloadingProvider downloadingProvider,
            MatchStarterSettings matchStarterSettings,
			IInstantiator instantiator)
		{
            this._downloadingProvider = downloadingProvider;
            _matchStarterSettings = matchStarterSettings;
			_instantiator = instantiator;
		}

		public async UniTask<MatchStarter> LoadAsync(bool autoComplete = true)
		{
			IsLoading = true;

			DestroyAndRelease();

            await _downloadingProvider.DownloadAssetWithLabelIfRequiredAsync(_matchStarterSettings.MatchStarterLabel);
			await InitLocationHandle();

			_currentLocation = _instantiator.InstantiatePrefab(_locationHandle.Value.Result);

			var location = _currentLocation.transform.GetComponent<MatchStarter>();

			if (location)
				await UniTask.WaitUntil(() => location.IsLoaded);

			if (autoComplete)
			{
				_locationLoaded.OnNext(Unit.Default);
				IsLoading = false;
			}

			return location;
		}

//		public UniTask CompleteAsync()
//		{
//			if (IsLoading)
//			{
//				_locationLoaded.OnNext(Unit.Default);
//				IsLoading = false;
//			}
//
//			return UniTask.CompletedTask;
//		}

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

			_locationHandle = _matchStarterSettings.MatchStarterPrefabReference.LoadAssetAsync();
			
			await _locationHandle.Value;
		}
		
		//Не тестировал
		public void DestroyAndRelease()
		{
			if (_currentLocation != null)
			{
				Object.Destroy(_currentLocation);
			}

			if (_locationHandle.HasValue )//&& _locationHandle.Value.IsDone && _locationHandle.Value.IsValid())
				UnityEngine.AddressableAssets.Addressables.Release(_locationHandle.Value);

			_locationHandle = null;
		}
	}
}