using Cysharp.Threading.Tasks;
using MainMenuFarm;
using UI.MainMenuWindow;
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
        private void Construct(IWindowsManager windowsManager,
            IMainMenuFarmLoader mainMenuFarmLoader)
        {
            _windowsManager = windowsManager;
            _mainMenuFarmLoader = mainMenuFarmLoader;
        }

        [Inject]
        private void Construct(IMainMenuFarmLoader mainMenuFarmLoader)
        {
            _mainMenuFarmLoader = mainMenuFarmLoader;
        }

        public override async UniTask LoadStep()
        {
            var mainMenuWindowController = await _windowsManager.OpenAsync<MainMenuWindowController>();
            var mainMenuFarm = await _mainMenuFarmLoader.LoadAsync();
        }
    }
}