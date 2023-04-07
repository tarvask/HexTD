using Cysharp.Threading.Tasks;
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

        public override int StepWeight => 2;

        [Inject]
        private void Construct(IWindowsManager windowsManager)
        {
            _windowsManager= windowsManager;
        }

        public override async UniTask LoadStep()
        {
	        var mainMenuWindowController = await _windowsManager.OpenAsync<MainMenuWindowController>();
        }
    }
}