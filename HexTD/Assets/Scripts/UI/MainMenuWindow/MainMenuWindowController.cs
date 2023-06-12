using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using WindowSystem.Controller;
using Extensions;
using Game;
using UI.ShopWindow;
using UI.InventoryWindow;
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

			View.InventoryButtonClick
				.Subscribe(ShowInentory)
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
			if (WindowsManager.IsOpen<InventoryWindowController>())
			{
				WindowsManager.CloseAsync(WindowsManager.GetOpened<InventoryWindowController>()).Forget();
			}

			WindowsManager.OpenAsync<MarketWindowController>();
		}

		private void ShowInentory()
        {
			if (WindowsManager.IsOpen<MarketWindowController>())
			{
				WindowsManager.CloseAsync(WindowsManager.GetOpened<MarketWindowController>()).Forget();
			}

			WindowsManager.OpenAsync<InventoryWindowController>();
        }

		private void CloseWindows()
        {
			WindowsManager.CloseAsync(this).Forget();

			if (WindowsManager.IsOpen<MarketWindowController>())
			{
				WindowsManager.CloseAsync(WindowsManager.GetOpened<MarketWindowController>()).Forget();
			}

			if (WindowsManager.IsOpen<InventoryWindowController>())
            {
				WindowsManager.CloseAsync(WindowsManager.GetOpened<InventoryWindowController>()).Forget();
            }
		}
	}
}