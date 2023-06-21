using Cysharp.Threading.Tasks;
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

        public override int StepWeight => 2;

        [Inject]
        private void Construct(IWindowsManager windowsManager)
        {
            _windowsManager = windowsManager;
        }

        public override async UniTask LoadStep()
        {
	        await _windowsManager.OpenAsync<AuthorizationWindowController>();
        }
    }
}