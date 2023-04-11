using Match;
using MatchStarter;

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
			_matchSettingsProvider.Settings = new MatchSettings(isMultiPlayer);

			var matchStarter = await _matchStarterLoader.LoadAsync();
		}
	}
}