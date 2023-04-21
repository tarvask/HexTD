using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using WindowSystem.Controller;
using Extensions;
using Game;
using UniRx;

namespace UI.MainMenuWindow
{
	public class MainMenuWindowController : LoadableWindowController<MainMenuWindowView>
	{
		private readonly GameController _gameController;

		public MainMenuWindowController(
			GameController gameController
		)
		{
			_gameController = gameController;
		}

		protected override void DoInitialize()
		{
			View.SinglePlayerBattleRunClick
				.Subscribe(RunSinglePlayerBattle)
				.AddTo(View);

			View.MultiPlayerBattleRunClick
				.Subscribe(RunMultiPlayerBattle)
				.AddTo(View);
		}

		protected override UniTask DoShowAsync(bool animated = true)
		{
			return TestLoad();
		}

		private async UniTask TestLoad()
		{
			await Task.Delay(300);
			//View.TestShowLoaded();
		}

		private void RunSinglePlayerBattle()
		{
			WindowsManager.CloseAsync(this).Forget();

			_gameController.RunBattle(false);
		}

		private void RunMultiPlayerBattle()
		{
			WindowsManager.CloseAsync(this).Forget();

			_gameController.RunBattle(true);
		}
	}
}