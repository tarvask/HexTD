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
		private readonly IInstantiator instantiator;
		private readonly Canvas canvas;
		private readonly AddressableWindowViewContainer viewContainer;
		private readonly AssetReferenceContainer<string> viewAssetsReferenceCache;

		public AddressableWindowViewFactory(
			IInstantiator instantiator,
			Canvas canvas,
			AddressableWindowViewContainer viewContainer)
		{
			this.instantiator = instantiator;
			this.canvas = canvas;
			this.viewContainer = viewContainer;
			viewAssetsReferenceCache = new AssetReferenceContainer<string>(viewContainer.WindowAssets.Count);
		}

		public async UniTask<OperationResult<TWindowView>> CreateAsync<TWindowView>()
			where TWindowView : WindowViewBase
		{
			var referenceInfo = viewContainer.GetReferenceInfo<TWindowView>();

			var assetHandle =
				UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(referenceInfo.AssetReference);

			await assetHandle;

			if (assetHandle.Status != AsyncOperationStatus.Succeeded)
				return OperationResult<TWindowView>.Failed(ex: assetHandle.OperationException);

			await UniTask.WaitForEndOfFrame();

			var uiElement =
				instantiator.InstantiatePrefabForComponent<TWindowView>(assetHandle.Result, canvas.transform);
			uiElement.gameObject.SetActive(false);

			viewAssetsReferenceCache.CacheInMemory(referenceInfo.AssetReference.AssetGUID, assetHandle);

			return OperationResult<TWindowView>.Success(uiElement);
		}

		public void Release(WindowViewBase windowView)
		{
			Object.Destroy(windowView.gameObject);

			var referenceInfo = viewContainer.GetReferenceInfo(windowView.GetType());

			if (!referenceInfo.CacheInMemory)
			{
				viewAssetsReferenceCache.RemoveFromMemory(referenceInfo.AssetReference.AssetGUID);
//                Log.Debug(LogTag.UI, $"Release {windowView.GetType().Name}", this);
				Debug.Log($"Release {windowView.GetType().Name}");
			}
		}
	}
}