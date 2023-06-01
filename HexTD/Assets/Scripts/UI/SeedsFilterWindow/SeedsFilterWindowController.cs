using Cysharp.Threading.Tasks;
using Extensions;
using UniRx;
using WindowSystem.Controller;

namespace UI.SeedsFilterWindow
{
    public class SeedsFilterWindowController : LoadableWindowController<SeedsFilterWindowView>
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