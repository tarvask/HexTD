using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using WindowSystem.Controller;
using Extensions;
using MatchStarter;
using UniRx;

namespace UI.MainMenuWindow
{
	public class MainMenuWindowController : LoadableWindowController<MainMenuWindowView>
	{
		private readonly IMatchStarterLoader _matchStarterLoader;

		public MainMenuWindowController(
			IMatchStarterLoader matchStarterLoader
		)
		{
			_matchStarterLoader = matchStarterLoader;
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

			await _matchStarterLoader.LoadAsync();
		}
	}
}