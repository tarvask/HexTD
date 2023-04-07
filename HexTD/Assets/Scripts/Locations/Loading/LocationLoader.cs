using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using Object = UnityEngine.Object;

namespace Locations.Loading
{
	public class LocationLoader : ILocationLoader
	{
//        private readonly AssetsDownloadingProvider downloadingProvider;
		private readonly LocationDB _locationDb;
		private readonly IInstantiator _instantiator;
		private readonly Subject<string> _locationLoaded = new Subject<string>();

		private readonly Dictionary<string, AsyncOperationHandle<GameObject>> _handlesCache =
			new Dictionary<string, AsyncOperationHandle<GameObject>>();

		private readonly List<string> _locationsToCache = new List<string> { "new_map" };

		private AsyncOperationHandle<GameObject> _locationHandle;
		private GameObject _currentLocation;
		private string _currentLocationId;

		public IObservable<string> LocationLoaded => _locationLoaded;
		public bool IsLoading { get; private set; }
		public GameObject CurrentLocation => _currentLocation;
		public string CurrentLocationId => _currentLocationId;

		public LocationLoader(
//            AssetsDownloadingProvider downloadingProvider,
			LocationDB locationDb,
			IInstantiator instantiator)
		{
//            this.downloadingProvider = downloadingProvider;
			_locationDb = locationDb;
			_instantiator = instantiator;
		}

		public async UniTask LoadAsync(string locationId, bool autoComplete = true)
		{
			IsLoading = true;

			DestroyCurrentLocation(locationId);

//            await downloadingProvider.DownloadAssetWithLabelIfRequiredAsync(locationDb.GetLabel(locationId));
			await InitLocationHandle(locationId);

			_currentLocation = _instantiator.InstantiatePrefab(_locationHandle.Result);
			_currentLocationId = locationId;

			var location = _currentLocation.transform.GetComponentInChildren<Location>();

			if (location)
				await UniTask.WaitUntil(() => location.IsLoaded);

			if (autoComplete)
			{
				_locationLoaded.OnNext(locationId);
				IsLoading = false;
			}
		}

		public UniTask CompleteAsync()
		{
			if (IsLoading)
			{
				_locationLoaded.OnNext(_currentLocationId);
				IsLoading = false;
			}

			return UniTask.CompletedTask;
		}

		public void HandleCityLocationLoading(string locationId)
		{
			_currentLocationId = locationId;
			IsLoading = true;

			CompleteAsync();
		}

		private void DestroyCurrentLocation(string locationId)
		{
			if (_currentLocation != null)
			{
				Object.Destroy(_currentLocation);
			}

			if (_locationHandle.IsDone && _locationHandle.IsValid() && !_handlesCache.ContainsValue(_locationHandle))
				Addressables.Release(_locationHandle);
		}

		private async Task InitLocationHandle(string locationId)
		{
			if (_handlesCache.TryGetValue(locationId, out var handle))
			{
				_locationHandle = handle;
				return;
			}

			_locationHandle = _locationDb.Find(locationId).LoadAssetAsync();
			await _locationHandle;

			if (_locationsToCache.Contains(locationId))
			{
				_handlesCache.Add(locationId, _locationHandle);
			}
		}
	}
}