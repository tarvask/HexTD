using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Locations.Loading
{
	public interface ILocationLoader
	{
		bool IsLoading { get; }
		string CurrentLocationId { get; }
		GameObject CurrentLocation { get; }

		IObservable<string> LocationLoaded { get; }

		UniTask LoadAsync(string locationId, bool autoComplete = true);
		UniTask CompleteAsync();
		void HandleCityLocationLoading(string locationId);
	}
}