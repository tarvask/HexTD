using System.Collections.Generic;
using Configs.Constants;
using MapEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HexSystem
{
	public class HexGridModel
	{
		private readonly EditorHexFabric _hexFabric;
		private readonly Layout _layout;
		private readonly IDictionary<int, HexModel> _hexModels;
		private readonly IDictionary<int, HexObject> _hexObjects;

		public HexGridModel(EditorHexFabric hexFabric, Layout layout)
		{
			_hexFabric = hexFabric;
			_layout = layout;
			_hexModels = new Dictionary<int, HexModel>();
			_hexObjects = new Dictionary<int, HexObject>();
		}

		public HexModel GetHexModel(Hex2d hex) => _hexModels.ContainsKey(hex.GetHashCode())
			? _hexModels[hex.GetHashCode()]
			: null;

		public HexModel CreateHex(Hex2d hexPosition, List<(string, string)> parameters)
		{
			var hexModel = new HexModel(hexPosition, 0, parameters);
			_hexModels.Add(hexModel.GetHashCode(), hexModel);

			Vector3 spawnPosition = _layout.ToPlane(hexPosition);
			HexObject hexInstance = _hexFabric.CreateHexObject(hexModel, spawnPosition);
			_hexObjects.Add(hexModel.GetHashCode(), hexInstance);

			return hexModel;
		}

		public void CreateHex(HexModel hexModel)
		{
			var copiedHexModel = new HexModel(hexModel);
			_hexModels.Add(copiedHexModel.GetHashCode(), copiedHexModel);

			Vector3 spawnPosition = _layout.ToPlane(hexModel.Q, hexModel.R, hexModel.Height);
			HexObject hexInstance = _hexFabric.CreateHexObject(copiedHexModel, spawnPosition);
			_hexObjects.Add(copiedHexModel.GetHashCode(), hexInstance);
		}

		public HexObject GetHexagonInstance(HexModel hexModel)
		{
			return _hexObjects[hexModel.GetHashCode()];
		}
		
		public void RemoveHexFromHexGrid(Hex2d hex)
		{
			var hexModel = GetHexModel(hex);
			if(hexModel == null)
				return;
			
			RemoveHexFromHexGrid(hexModel);
		}
		
		public void RemoveHexFromHexGrid(HexModel hexModel)
		{
			if(!_hexObjects.Remove(hexModel.GetHashCode(), out var hex))
				return;
            
			Object.Destroy(hex.gameObject);
			_hexModels.Remove(hexModel.GetHashCode());
		}

		public List<HexModel> GetAllHexes()
		{
			return new List<HexModel>(_hexModels.Values);
		}

		public void Clear()
		{
			foreach (var hexagon in _hexObjects)
			{
				Object.Destroy(hexagon.Value.gameObject);
			}   
			
			_hexObjects.Clear();
			_hexModels.Clear();
		}

		public bool GetHexIsBlocker(Hex2d hex)
		{
			if (!_hexModels[hex.GetHashCode()].Data
				    .TryGetValue(HexParamsNameConstants.IsBlockerParam, out var isBlocker))
			{
				return false;
			}

			return bool.Parse(isBlocker);
		}

		public void SetHexIsBlocker(Hex2d hex, bool isBlocker)
		{
			int hexHashCode = hex.GetHashCode();
			
			if (!_hexModels[hexHashCode].Data.ContainsKey(HexParamsNameConstants.IsBlockerParam))
			{
				_hexModels[hexHashCode].Data.Add(HexParamsNameConstants.IsBlockerParam,isBlocker.ToString());
			}
			else
			{
				_hexModels[hexHashCode].Data[HexParamsNameConstants.IsBlockerParam] = isBlocker.ToString();
			}

			_hexObjects[hexHashCode].SetIsBlocker(isBlocker);
		}
	}
}