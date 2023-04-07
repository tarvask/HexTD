using TMPro;
using UI.Custom;
using UnityEngine;
using WindowSystem.View;

namespace UI.Loading_Window
{
	public class LoadingWindowView : WindowViewBase
	{
		private const string InitialSkinName = "default";
		private const string AnimationName = "Animation";

		[SerializeField] private TextMeshProUGUI version;

//        [SerializeField] private SkeletonAnimation loadingScreenSkeleton;
		[SerializeField] private AnimatedLoadingBar loadingBar;
		[SerializeField] private AnimatedLoadingBar downloadingBar;
		[SerializeField] private TextMeshProUGUI downloadingSizeProgress;

		public bool InProgress => loadingBar.InProgress;

		public void ClearView()
		{
			loadingBar.SetProgress(0);
			downloadingBar.SetProgress(0);
			downloadingSizeProgress.text = string.Empty;
		}

		public void Configure(string versionText)
		{
			version.text = versionText;

//            ConfigureLoadingScreenSkeleton(skeletonDataAsset);
		}

		public void SetActiveDownloading(bool isActive) => downloadingBar.SetActive(isActive);
		public void SetActiveLoading(bool isActive) => loadingBar.SetActive(isActive);

		public void SetLoadingProgress(float progress) => loadingBar.AnimateProgress(progress);
		public void SetDownloadingProgress(float progress) => downloadingBar.SetProgress(progress);
		public void SetDownloadingSize(string downloadSizeText) => downloadingSizeProgress.text = downloadSizeText;
	}
}