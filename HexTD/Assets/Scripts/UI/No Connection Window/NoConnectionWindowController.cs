using Cysharp.Threading.Tasks;
using Extensions;
using UniRx;
using WindowSystem.Controller;

namespace UI.No_Connection_Window
{
	public class NoConnectionWindowController : LoadableWindowController<NoConnectionWindowView>
	{
		protected override void DoInitialize()
		{
			View.CloseClick
				.Subscribe(CloseWindow)
				.AddTo(View);
		}

		private void CloseWindow() => WindowsManager.CloseAsync(this).Forget();
	}
}