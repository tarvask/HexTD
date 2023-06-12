using Cysharp.Threading.Tasks;
using Extensions;
using UniRx;
using WindowSystem.Controller;

namespace UI.GreenhousesFilterWindow
{
    public class GreenhousesFilterWindowController : LoadableWindowController<GreenhousesFilterWindowView>
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