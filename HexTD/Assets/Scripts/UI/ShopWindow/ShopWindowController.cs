using Cysharp.Threading.Tasks;
using Extensions;
using UI.ShopwWindow;
using UniRx;
using WindowSystem.Controller;

namespace UI.ShopWindow
{
    public class ShopWindowController : LoadableWindowController<ShopWindowView>
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