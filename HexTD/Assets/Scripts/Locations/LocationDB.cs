using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;

namespace Locations
{
	public class LocationDB
	{
		[Serializable]
		public class Settings
		{
			public List<LocationSettings> locations;
		}

		[Serializable]
		public class LocationSettings
		{
			public string id;
			public AssetReferenceGameObject locationReference;
			public AssetLabelReference locationLabel;
		}

		private readonly Settings settings;

		public LocationDB(Settings settings)
		{
			this.settings = settings;
		}

		public AssetReferenceGameObject Find(string id) =>
			settings.locations.First(x => x.id == id).locationReference;

		public AssetLabelReference GetLabel(string id) =>
			settings.locations.First(x => x.id == id).locationLabel;
	}
}