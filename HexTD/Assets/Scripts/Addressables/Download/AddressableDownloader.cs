using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Logger;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Addressables.Download
{
	public static class AddressableDownloader
	{
		public static void ClearLabel() => Caching.ClearCache();

		public static async UniTask<bool> IsRequiredDownloadAsync(AssetLabelReference label)
		{
			var isValidKey = label.RuntimeKeyIsValid();
			var sizeRequest = UnityEngine.AddressableAssets.Addressables.GetDownloadSizeAsync(label);
			await sizeRequest.Task;
			return isValidKey && sizeRequest.Status == AsyncOperationStatus.Succeeded && sizeRequest.Result > 0;
		}

		public static async UniTask<DownloadOperationResult> DownloadAsync(
			IReadOnlyList<AssetLabelReference> assetLabel,
			DownloadType downloadType,
			IDownloadProgress progress = null)
		{
			if (downloadType == DownloadType.Sequential)
				return await InternalDownloadSequentialAsync(assetLabel, progress);

			if (downloadType == DownloadType.Parallel)
				return await InternalDownloadParallelAsync(assetLabel, progress);

			throw new Exception($"Not supported download type : {downloadType.ToString()}");
		}

		public static async UniTask<DownloadOperationResult> DownloadAsync(
			AssetLabelReference assetLabel,
			IDownloadProgress progress = null)
		{
			var stringLabel = assetLabel.labelString;

			if (!await IsRequiredDownloadAsync(assetLabel))
			{
				Log.Trace(LogTag.Addressable,
					$"<color=green>Assets with label {stringLabel} already downloaded!</color>",
					nameof(AddressableDownloader));
				return new DownloadOperationResult(stringLabel, true);
			}

//            GameflowAnalytics.SendAssetsLoadEvent(assetLabel.labelString, AnalyticsAssetLoadingStatus.Start);

			var downloadingOperation = UnityEngine.AddressableAssets.Addressables.DownloadDependenciesAsync(assetLabel);
			var downloadTime = 0f;

			while (!downloadingOperation.IsDone)
			{
				var operation = downloadingOperation.GetDownloadStatus();
				progress?.Report(operation.DownloadedBytes, operation.TotalBytes);
				downloadTime += Time.deltaTime;
				await UniTask.Yield();
			}

			var isComplete = IsOperationValidAndComplete(downloadingOperation);
			var downloadStatus = downloadingOperation.GetDownloadStatus();
			var downloadedBytes = downloadStatus.DownloadedBytes;
			var totalBytes = downloadStatus.TotalBytes;
			var exception = downloadingOperation.OperationException;

//            GameflowAnalytics.SendAssetsLoadEvent(assetLabel.labelString,
//                isComplete ? AnalyticsAssetLoadingStatus.Done : AnalyticsAssetLoadingStatus.Failed);

			Log.Trace(LogTag.Addressable, isComplete
				? $"<color=green>{stringLabel} is downloaded!</color>"
				: $"<color=red>{stringLabel} downloading failed!</color>", nameof(AddressableDownloader));

			return new DownloadOperationResult(stringLabel, isComplete, downloadedBytes, totalBytes, downloadTime,
				exception);
		}

		// public static void Log(string mes) => Debug.Log($"[AssetDownloader]: {mes}");

		private static async UniTask<DownloadOperationResult> InternalDownloadParallelAsync(
			IReadOnlyList<AssetLabelReference> assetLabel,
			IDownloadProgress progress)
		{
			var isComplete = true;
			var downloadTime = 0f;
			var downloadedBytes = 0L;
			var totalBytes = 0L;
			Exception exception = null;

			var taskList = new UniTask<DownloadOperationResult>[assetLabel.Count];
			var parallelProgress = new DownloadProgress[assetLabel.Count];

			for (var i = 0; i < assetLabel.Count; i++)
			{
				var labelReference = assetLabel[i];
				parallelProgress[i] = new DownloadProgress();
				taskList[i] = DownloadAsync(labelReference, parallelProgress[i]);
			}

			var whenAll = UniTask.WhenAll(taskList);

			if (!ReferenceEquals(progress, null))
			{
				do
				{
					var downloaded = 0L;
					var total = 0L;

					foreach (var downloadProgress in parallelProgress)
					{
						downloaded += downloadProgress.DownloadedBytes;
						total += downloadProgress.TotalBytes;
					}

					progress.Report(downloaded, total);
					await UniTask.Yield();
				} while (whenAll.Status == UniTaskStatus.Pending);
			}

			var results = await whenAll;

			foreach (var result in results)
			{
				if (!result.IsComplete)
					exception = result.Error;

				isComplete &= result.IsComplete;
				downloadTime = Mathf.Max(downloadTime, result.Time);
				downloadedBytes += result.DownloadedBytes;
				totalBytes += result.TotalBytes;
			}

			return new DownloadOperationResult(assetLabel, isComplete, downloadedBytes, totalBytes, downloadTime,
				exception);
		}

		private static async UniTask<DownloadOperationResult> InternalDownloadSequentialAsync(
			IReadOnlyList<AssetLabelReference> assetLabel,
			IDownloadProgress progress)
		{
			var isComplete = true;
			var downloadTime = 0f;
			var downloadedBytes = 0L;
			var totalBytes = 0L;
			Exception exception = null;

			foreach (var labelReference in assetLabel)
			{
				var result = await DownloadAsync(labelReference, progress);

				if (!result.IsComplete)
					exception = result.Error;

				isComplete &= result.IsComplete;
				downloadTime += result.Time;
				downloadedBytes += result.DownloadedBytes;
				totalBytes += result.TotalBytes;
			}

			return new DownloadOperationResult(assetLabel, isComplete, downloadedBytes, totalBytes, downloadTime,
				exception);
		}

		private static bool IsOperationValidAndComplete(AsyncOperationHandle downloadingOperation) =>
			downloadingOperation.IsDone &&
			downloadingOperation.IsValid() &&
			downloadingOperation.Status == AsyncOperationStatus.Succeeded;

		private class DownloadProgress : IDownloadProgress
		{
			public long DownloadedBytes { get; private set; }
			public long TotalBytes { get; private set; }

			public void Report(long downloadedBytes, long totalBytes)
			{
				DownloadedBytes = downloadedBytes;
				TotalBytes = totalBytes;
			}
		}
	}
}