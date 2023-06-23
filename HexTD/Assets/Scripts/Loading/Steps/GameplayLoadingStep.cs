using Cysharp.Threading.Tasks;
using MainMenuFarm;
using UI.AuthorizationWindow;
using UnityEngine;
using WindowSystem;
using Zenject;

namespace Loading.Steps
{
    [CreateAssetMenu(menuName = "Game/Loading/Gameplay Step")]
    public class GameplayLoadingStep : GameLoadingStep
    {
	    private IWindowsManager _windowsManager;
        private IMainMenuFarmLoader _mainMenuFarmLoader;

        public override int StepWeight => 2;

        [Inject]
        private void Construct(
            IWindowsManager windowsManager,
            IMainMenuFarmLoader mainMenuFarmLoader)
        {
            _windowsManager = windowsManager;
            _mainMenuFarmLoader = mainMenuFarmLoader;
        }

        public override async UniTask LoadStep()
        {
	        await _windowsManager.OpenAsync<AuthorizationWindowController>();
            await _mainMenuFarmLoader.LoadAsync();
        }
    }
}
