using System;
using UnityEngine.AddressableAssets;

namespace MatchDataBase
{
	public class MatchDataBase
	{
		[Serializable]
		public class Settings
		{
			public MatchStarterSettings MatchStarterSettings;
		}

		private readonly Settings _settings;

		public MatchDataBase(Settings settings)
		{
			_settings = settings;
		}

		public MatchStarterSettings MatchStarterSettings => _settings.MatchStarterSettings;
	}
	
	[Serializable]
	public class MatchStarterSettings
	{
		public AssetReferenceGameObject MatchStarterPrefabReference;
		public AssetLabelReference MatchStarterLabel;
	}
}