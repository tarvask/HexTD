using Addressables.Download;
using Cysharp.Threading.Tasks;
using Logger;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Addressables
{
    public class AddressableBackgroundWorker : MonoBehaviour
    {
        [SerializeField] private DownloadType downloadType = DownloadType.Sequential;
        [SerializeField] private AssetLabelReference[] preloadedLabels;

        private void Start() => LoadAddressables().Forget();

        private async UniTaskVoid LoadAddressables()
        {
            var result = await AddressableDownloader.DownloadAsync(preloadedLabels, downloadType);
            Log.Trace(LogTag.Addressable, $"{downloadType} loading complete!", nameof(AddressableDownloader));
            Log.Trace(LogTag.Addressable, result, nameof(AddressableDownloader));
        }
    }
}