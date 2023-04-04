using System.Collections.Generic;
using HexSystem;
using Newtonsoft.Json;
using PathSystem;

namespace MapEditor
{
	public class LevelMapModel
	{
		[JsonProperty("Hexes")]
		public List<HexModel> HexModels;
		[JsonProperty("Paths")]
		public List<PathData.SavePathData> PathDatas;
	}
}