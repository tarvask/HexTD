using Addressables;
using Cysharp.Threading.Tasks;
using Tools;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace WindowSystem.View.Factories.Addressable
{
	public class AddressableWindowViewFactory : IWindowViewFactory
	{
		private readonly IInstantiator _instantiator;
		private readonly Canvas _canvas;
		private readonly AddressableWindowViewContainer _viewContainer;
		private readonly AssetReferenceContainer<string> _viewAssetsReferenceCache;

		public AddressableWindowViewFactory(
			IInstantiator instantiator,
			Canvas canvas,
			AddressableWindowViewContainer viewContainer)
		{
			_instantiator = instantiator;
			_canvas = canvas;
			_viewContainer = viewContainer;
			_viewAssetsReferenceCache = new AssetReferenceContainer<string>(viewContainer.WindowAssets.Count);
		}

		public async UniTask<OperationResult<TWindowView>> CreateAsync<TWindowView>()
			where TWindowView : WindowViewBase
		{
			var referenceInfo = _viewContainer.GetReferenceInfo<TWindowView>();

			var assetHandle =
				UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(referenceInfo.AssetReference);

			await assetHandle;

			if (assetHandle.Status != AsyncOperationStatus.Succeeded)
				return OperationResult<TWindowView>.Failed(ex: assetHandle.OperationException);

			await UniTask.WaitForEndOfFrame();

			var uiElement =
				_instantiator.InstantiatePrefabForComponent<TWindowView>(assetHandle.Result, _canvas.transform);
			uiElement.gameObject.SetActive(false);

			_viewAssetsReferenceCache.CacheInMemory(referenceInfo.AssetReference.AssetGUID, assetHandle);

			return OperationResult<TWindowView>.Success(uiElement);
		}

		public void Release(WindowViewBase windowView)
		{
			Object.Destroy(windowView.gameObject);

			var referenceInfo = _viewContainer.GetReferenceInfo(windowView.GetType());

			if (!referenceInfo.CacheInMemory)
			{
				_viewAssetsReferenceCache.RemoveFromMemory(referenceInfo.AssetReference.AssetGUID);
//                Log.Debug(LogTag.UI, $"Release {windowView.GetType().Name}", this);
				Debug.Log($"Release {windowView.GetType().Name}");
			}
		}
	}
}