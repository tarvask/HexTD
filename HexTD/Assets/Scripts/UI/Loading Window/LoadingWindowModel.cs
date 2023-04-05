using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using WindowSystem.Model;

namespace UI.Loading_Window
{
	[CreateAssetMenu(menuName = "Game/Window/Models/Loading Model")]
	public class LoadingWindowModel : WindowModelBase
	{
//        [SerializeField] private AssetReference[] skeletonDataAssetReferences;

		public string VersionInfo => $"Version: v{BuildInfo.FullBuildVersion} Bundle: {BuildInfo.BundleVersionCode}";

		public AssetReference GetWindowAssetReference()
		{
//            var appData = DataContainer.GetData<ApplicationSettingsData>();
//            var index = appData.LoadingImageIndex % skeletonDataAssetReferences.Length;
//            var assetReference = skeletonDataAssetReferences[index];
//            
//            appData.IncreaseLoadingImageIndex();

			return null;
		}

		public string GetDownloadSizeInfo(long downloadedBytes, long totalBytes)
		{
			var downloadedMegabytes = ToMegabytes(downloadedBytes);
			var totalMegabytes = ToMegabytes(totalBytes);

//            var downloadingKey = LocaleDBEntry.general.downloading;
//            var downloadText = LocaleDB.Generals.Find(downloadingKey)?.Text;
//            if (string.IsNullOrEmpty(downloadText))
//                downloadText =
//                    $"{Localization.MarkAsErrorText(downloadingKey.ToString())}... {downloadedMegabytes}M/{totalMegabytes}M";
//            var downloadSizeText = string.Format(downloadText, downloadedMegabytes, totalMegabytes);
			var downloadSizeText = string.Format("Downloaded:", downloadedMegabytes, totalMegabytes);

			return (downloadSizeText);

			double ToMegabytes(long size) => Math.Round(size / 1000000f, 2);
		}
	}
}