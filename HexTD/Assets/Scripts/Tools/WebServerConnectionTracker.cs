using Cysharp.Threading.Tasks;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.Networking;

namespace Tools
{
    public static class WebServerConnectionTracker
    {
        private const string DefaultServerUrl = "";

        private static string AddressableServerUrl
        {
            get
            {
#if UNITY_EDITOR
                var addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
                var assetBundleUrl = addressableAssetSettings.profileSettings.EvaluateString(
                    addressableAssetSettings.activeProfileId,
                    addressableAssetSettings.profileSettings.GetValueByName(addressableAssetSettings.activeProfileId,
                        AddressableAssetSettings.kRemoteLoadPath));
#else
                var assetBundleUrl = BuildInfo.ServerUrl;
#endif
                return assetBundleUrl;
            }
        }

        public static UniTask<bool> IsServerReachable(string url)
        {
            var request = UnityWebRequest.Get(url).SendWebRequest();
            var completion = new UniTaskCompletionSource<bool>();
            //request.completed += operation =>
            //{ 
            //    var result = !((UnityWebRequestAsyncOperation)operation).webRequest.isNetworkError &&
            //                 Application.internetReachability != NetworkReachability.NotReachable;
            //    completion.TrySetResult(result);
            //};
            completion.TrySetResult(true);
            return completion.Task;
        }

        public static UniTask<bool> IsInternetReachable() => new UniTask<bool>(true);//IsServerReachable(DefaultServerUrl);
        public static UniTask<bool> IsAddressableServerReachable() => IsServerReachable(AddressableServerUrl);
    }
}