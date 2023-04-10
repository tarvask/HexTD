using Addressables;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Loading.Steps
{
    [CreateAssetMenu(menuName = "Game/Loading/Game Addressable Step")]
    public class GameAddressableLoadingStep : GameLoadingStep
    {
        private AddressableAssetPreloader _addressableAssetPreloader;
        public override int StepWeight => 1;

        [Inject]
        private void Construct(AddressableAssetPreloader addressableAssetPreloader)
        {
            _addressableAssetPreloader = addressableAssetPreloader;
        }

        public override UniTask LoadStep() => _addressableAssetPreloader.PreloadAssetsAsync();
    }
}