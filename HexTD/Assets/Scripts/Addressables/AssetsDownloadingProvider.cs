using Addressables.Download;
using Cysharp.Threading.Tasks;
using Tools;
using UI.Loading_Window;
using UI.No_Connection_Window;
using UnityEngine.AddressableAssets;
using WindowSystem;

namespace Addressables
{
    public class AssetsDownloadingProvider
    {
        private const int ReconnectionDelayMs = 2000;
        private readonly IWindowsManager windowsManager;

        public AssetsDownloadingProvider(IWindowsManager windowsManager)
        {
            this.windowsManager = windowsManager;
        }

        public async UniTask DownloadAssetWithLabelIfRequiredAsync(AssetLabelReference label)
        {
            if (!await AddressableDownloader.IsRequiredDownloadAsync(label))
                return;

            var selfControlledProcess = !windowsManager.IsOpen<LoadingWindowController>();
            var loadingController = await windowsManager.OpenSingleAsync<LoadingWindowController>();
            loadingController.SetActiveDownloading(true);

            do
            {
                if (await WebServerConnectionTracker.IsAddressableServerReachable())
                {
                    var result = await AddressableDownloader.DownloadAsync(label, loadingController);

                    if (result.IsComplete)
                        break;
                }

                await windowsManager.OpenSingleAsync<NoConnectionWindowController>()
                    .WaitUntilClosedAsync();

                await UniTask.Delay(ReconnectionDelayMs);
            } while (true);

            if (selfControlledProcess)
                await windowsManager.CloseAsync(loadingController);
            else
                loadingController.SetActiveDownloading(false);
        }
    }
}