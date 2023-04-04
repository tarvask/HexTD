using System.Collections.Generic;
using MapEditor;
using UnityEngine;

namespace HexSystem
{
	public class HexGridModel
	{
		private readonly HexFabric _hexFabric;
		private readonly List<HexModel> _hexModels;
		private readonly IDictionary<HexModel, HexObject> _hexagons;

		public HexGridModel(HexFabric hexFabric)
		{
			_hexFabric = hexFabric;
			_hexModels = new List<HexModel>();
			_hexagons = new Dictionary<HexModel, HexObject>();
		}
		
		public HexModel GetData(Hex2d hex)
		{
			return _hexModels.Find(data => data.Position.Q == hex.Q && data.Position.R == hex.R);
		}

		public HexModel CreateHex(Hex2d hexPosition, List<(string, string)> parameters)
		{
			var hexModel = new HexModel(hexPosition, 0, parameters);
			_hexModels.Add(hexModel);

			HexObject hexInstance = _hexFabric.CreateHexObject(hexModel);
			_hexagons.Add(hexModel, hexInstance);

			return hexModel;
		}

		public void CreateHex(HexModel hexModel)
		{
			var copiedHexModel = new HexModel(hexModel);
			_hexModels.Add(copiedHexModel);

			HexObject hexInstance = _hexFabric.CreateHexObject(copiedHexModel);
			_hexagons.Add(copiedHexModel, hexInstance);
		}

		public HexObject GetHexagonInstance(HexModel hexModel)
		{
			return _hexagons[hexModel];
		}
		
		public void RemoveHexFromHexGrid(Hex2d hex)
		{
			var hexModel = GetData(hex);
			if(hexModel == null)
				return;
			
			RemoveHexFromHexGrid(hexModel);
		}
		
		public void RemoveHexFromHexGrid(HexModel hexModel)
		{
			if(!_hexagons.Remove(hexModel, out var hex))
				return;
            
			Object.Destroy(hex.gameObject);
			_hexModels.Remove(hexModel);
		}

		public List<HexModel> GetAllHexes()
		{
			return new List<HexModel>(_hexModels);
		}

		public void Clear()
		{
			foreach (var hexagon in _hexagons)
			{
				Object.Destroy(hexagon.Value.gameObject);
			}   
			
			_hexagons.Clear();
			_hexModels.Clear();
		}
	}
}