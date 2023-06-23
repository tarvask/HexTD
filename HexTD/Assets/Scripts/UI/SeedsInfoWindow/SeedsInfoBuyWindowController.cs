using Cysharp.Threading.Tasks;
using Extensions;
using UniRx;
using WindowSystem.Controller;

namespace UI.SeedsInfoBuyWindow
{
    public class SeedsInfoBuyWindowController : LoadableWindowController<SeedsInfoBuyWindowView>
    {
        protected override void DoInitialize()
        {
            View.CloseButtonClick
                .Subscribe(CloseWindow)
                .AddTo(View);
        }

        private void CloseWindow()
        {
            WindowsManager.CloseAsync(this).Forget();
        }
    }
}