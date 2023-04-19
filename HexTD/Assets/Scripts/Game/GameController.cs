using System;
using Cysharp.Threading.Tasks;
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

		private IDisposable _matchDisposable;

		public GameController(
			IMatchStarterLoader matchStarterLoader,
			MatchSettingsProvider matchSettingsProvider,
			IWindowsManager windowsManager)
		{
			_matchStarterLoader = matchStarterLoader;
			_matchSettingsProvider = matchSettingsProvider;
			_windowsManager = windowsManager;
		}

		public async void RunBattle(bool isMultiPlayer)
		{
			if (isMultiPlayer)
			{
				var photonConnectionService = new PhotonConnectionService().ConnectNow();
				await new WaitWhile(() => photonConnectionService.IsConnectedToRoom);
			}
			
			_matchSettingsProvider.Settings = new MatchSettings(isMultiPlayer);

			var matchStarter = await _matchStarterLoader.LoadAsync();

			_matchDisposable = matchStarter.OnQuitMatch.Subscribe(OnQuitMatchHandler);
		}

		private void OnQuitMatchHandler(Unit asd)
		{
			_matchDisposable.Dispose();
			
			_matchStarterLoader.DestroyAndRelease();
			
			_windowsManager.OpenAsync<MainMenuWindowController>();
		}
	}
}