using System.Collections.Generic;
using HexSystem;
using Match.Field;
using Newtonsoft.Json;
using PathSystem;

namespace MapEditor
{
	public class LevelMapModel
	{
		[JsonProperty("Hexes")]
		public List<HexModel> HexModels;
		[JsonProperty("Props")]
		public List<PropsModel> PropsModels;
		[JsonProperty("Paths")]
		public List<PathData.SavePathData> PathDatas;

		public List<FieldHex> GetFieldHexes()
		{
			List<FieldHex> fieldHexes = new List<FieldHex>(HexModels.Count);
			
			foreach (HexModel hexModel in HexModels)
			{
				fieldHexes.Add(new FieldHex(hexModel, FieldHexType.Free));
			}

			return fieldHexes;
		}

		public IReadOnlyList<PropsModel> GetFieldProps() => PropsModels;
	}
}