namespace Match
{
	public class MatchSettings
	{
		public bool IsMultiPlayer { get; }
//		public List<TowerType> TowersHand { get; }

		public MatchSettings(
			bool isMultiPlayer
		)
		{
			IsMultiPlayer = isMultiPlayer;
		}
	}
}