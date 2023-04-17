using Cysharp.Threading.Tasks;
using Match;
using MatchStarter;
using Services.PhotonRelated;
using UnityEngine;

namespace Game
{
	public class GameController
	{
		private readonly IMatchStarterLoader _matchStarterLoader;
		private readonly MatchSettingsProvider _matchSettingsProvider;

		public GameController(
			IMatchStarterLoader matchStarterLoader,
			MatchSettingsProvider matchSettingsProvider)
		{
			_matchStarterLoader = matchStarterLoader;
			_matchSettingsProvider = matchSettingsProvider;
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
		}
	}
}