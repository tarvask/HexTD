using Addressables;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Loading.Steps
{
    [CreateAssetMenu(menuName = "Game/Loading/Game Addressable Step")]
    public class GameAddressableLoadingStep : GameLoadingStep
    {
        private AddressableAssetPreloader assetPreloader;
        public override int StepWeight => 1;

        [Inject]
        private void Construct(AddressableAssetPreloader assetPreloader)
        {
            this.assetPreloader = assetPreloader;
        }

        public override UniTask LoadStep() => assetPreloader.PreloadAssetsAsync();
    }
}