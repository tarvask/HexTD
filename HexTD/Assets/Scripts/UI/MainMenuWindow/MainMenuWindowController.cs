using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using WindowSystem.Controller;
using Extensions;
using Locations.Loading;
using UniRx;

namespace UI.MainMenuWindow
{
	public class MainMenuWindowController : LoadableWindowController<MainMenuWindowView>
	{
		private readonly ILocationLoader _locationLoader;

		public MainMenuWindowController(
			ILocationLoader locationLoader
		)
		{
			_locationLoader = locationLoader;
		}

		protected override void DoInitialize()
		{
			View.BattleRunClick
				.Subscribe(BattleRun)
				.AddTo(View);
		}

		protected override UniTask DoShowAsync(bool animated = true)
		{
			return TestLoad();
		}

		private async UniTask TestLoad()
		{
			await Task.Delay(300);
			View.TestShowLoaded();
		}

		private async void BattleRun()
		{
			WindowsManager.CloseAsync(this).Forget();

			await _locationLoader.LoadAsync("qwe");
		}
	}
}