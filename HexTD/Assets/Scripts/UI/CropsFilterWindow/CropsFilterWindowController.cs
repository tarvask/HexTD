using Cysharp.Threading.Tasks;
using Extensions;
using UniRx;
using WindowSystem.Controller;

namespace UI.CropsFilterWindow
{
    public class CropsFilterWindowController : LoadableWindowController<CropsFilterWindowView>
    {
        protected override void DoInitialize()
        {
            View.ConfirmButtonClick
                .Subscribe(CloseWindow)
                .AddTo(View);
        }

        private void CloseWindow()
        {
            WindowsManager.CloseAsync(this).Forget();
        }
    }
}