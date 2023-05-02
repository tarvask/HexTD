using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using WindowSystem.Controller;
using Extensions;
using Game;
using UI.ShopWindow;
using UI.ShopwWindow;
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

			View.ShopButtonClick
				.Subscribe(ShowMarket)
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
			CloseWindows();

			_gameController.RunBattle(false);
		}

		private void RunMultiPlayerBattle()
		{
			CloseWindows();

			_gameController.RunBattle(true);
		}

		private void ShowMarket()
		{
			WindowsManager.OpenAsync<ShopWindowController>();
		}

		private void CloseWindows()
        {
			WindowsManager.CloseAsync(this).Forget();

			if (WindowsManager.IsOpen<ShopWindowController>())
			{
				WindowsManager.CloseAsync(WindowsManager.GetOpened<ShopWindowController>()).Forget();
			}
		}
	}
}