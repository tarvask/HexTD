using Cysharp.Threading.Tasks;
using Extensions;
using Tools.Disposing;
using UniRx;
using WindowSystem.Controller;

namespace UI.PlotsFilterWindow
{
    public class PlotsFilterWindowController : LoadableWindowController<PlotsFilterWindowView>
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