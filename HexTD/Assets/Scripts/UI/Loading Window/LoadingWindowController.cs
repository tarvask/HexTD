using System.Threading.Tasks;
using Addressables.Download;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WindowSystem.Controller;

namespace UI.Loading_Window
{
	public class LoadingWindowController : LoadableWindowController<LoadingWindowView>, IDownloadProgress
	{
		private readonly LoadingWindowModel _model;

//        private AsyncOperationHandle<SkeletonDataAsset> skeletonDataAssetHandle;

		public LoadingWindowController(LoadingWindowModel model)
		{
			_model = model;
		}

		public bool InProgress => View.InProgress;

		public void SetActiveDownloading(bool isActive)
		{
			View.SetActiveDownloading(isActive);
			View.SetActiveLoading(!isActive);
		}

		public void SetActiveLoading(bool isActive)
		{
			View.SetActiveLoading(isActive);
			View.SetActiveDownloading(!isActive);
		}

		public void SetProgress(float progress) => View.SetLoadingProgress(progress);

		public void Report(long downloadedBytes, long totalBytes)
		{
			var sizeText = _model.GetDownloadSizeInfo(downloadedBytes, totalBytes);
			View.SetDownloadingSize(sizeText);

			var converted = downloadedBytes / (double)totalBytes;
			var progress = Mathf.Clamp01((float)converted);
			View.SetDownloadingProgress(progress);
		}

		protected override async UniTask<bool> DoLoadAsync()
		{
//            var assetReference = model.GetWindowAssetReference();
//            skeletonDataAssetHandle = Addressables.LoadAssetAsync<SkeletonDataAsset>(assetReference);
//            await skeletonDataAssetHandle;
//            return skeletonDataAssetHandle.IsDone && skeletonDataAssetHandle.IsValid();

			await Task.CompletedTask;
			return true;
		}

		protected override void DoInitialize()
		{
			View.ClearView();
//            View.Configure(model.VersionInfo, skeletonDataAssetHandle.Result);
			View.Configure(_model.VersionInfo);
		}

//        protected override void DoDispose() => Addressables.Release(skeletonDataAssetHandle);
	}
}