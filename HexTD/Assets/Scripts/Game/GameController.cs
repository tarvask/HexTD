using System;
using Cysharp.Threading.Tasks;
using MainMenuFarm;
using Match;
using MatchStarter;
using Services.PhotonRelated;
using UI.MainMenuWindow;
using UniRx;
using UnityEngine;
using WindowSystem;

namespace Game
{
	public class GameController
	{
		private readonly IMatchStarterLoader _matchStarterLoader;
		private readonly MatchSettingsProvider _matchSettingsProvider;
		private readonly IWindowsManager _windowsManager;
		private readonly IMainMenuFarmLoader _mainMenuFarmLoader;
		
		private PhotonConnectionService _photonConnectionService;
		private IDisposable _onEndMatchSubscriptionDisposable;
		private IDisposable _onQuitMatchSubscriptionDisposable;

		public GameController(
			IMatchStarterLoader matchStarterLoader,
			MatchSettingsProvider matchSettingsProvider,
			IWindowsManager windowsManager,
			IMainMenuFarmLoader mainMenuFarmLoader
			)
		{
			_matchStarterLoader = matchStarterLoader;
			_matchSettingsProvider = matchSettingsProvider;
			_windowsManager = windowsManager;
			_mainMenuFarmLoader = mainMenuFarmLoader;
		}

		public async void RunBattle(bool isMultiPlayer)
		{
			if (isMultiPlayer)
			{
				_photonConnectionService = new PhotonConnectionService().ConnectNow();
				await new WaitWhile(() => _photonConnectionService.IsConnectedToRoom);
			}
			
			_matchSettingsProvider.Settings = new MatchSettings(isMultiPlayer);

			_mainMenuFarmLoader.DestroyAndRelease();
			var matchStarter = await _matchStarterLoader.LoadAsync();

			_onEndMatchSubscriptionDisposable = matchStarter.OnEndMatch.Subscribe(OnEndMatchHandler);
			_onQuitMatchSubscriptionDisposable = matchStarter.OnQuitMatch.Subscribe(OnQuitMatchHandler);
		}

		private void OnEndMatchHandler(Unit asd)
		{
			_onEndMatchSubscriptionDisposable.Dispose();
			_photonConnectionService?.Dispose();
		}

		private void OnQuitMatchHandler(Unit asd)
		{
			_onQuitMatchSubscriptionDisposable.Dispose();
			
			_matchStarterLoader.DestroyAndRelease();
			
			_windowsManager.OpenAsync<MainMenuWindowController>();
			_mainMenuFarmLoader.LoadAsync();
		}
	}
}