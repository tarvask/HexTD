using System.Collections.Generic;
using Configs.Constants;
using HexSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MapEditor
{
	public class EditorHexesModel
	{
		private readonly EditorHexObjectFabric _hexObjectFabric;
		private readonly Layout _layout;
		private readonly IDictionary<int, HexModel> _hexModels;
		private readonly IDictionary<int, HexObject> _hexObjects;

		public EditorHexesModel(EditorHexObjectFabric hexObjectFabric, Layout layout)
		{
			_hexObjectFabric = hexObjectFabric;
			_layout = layout;
			_hexModels = new Dictionary<int, HexModel>();
			_hexObjects = new Dictionary<int, HexObject>();
		}

		public HexModel GetHexModel(Hex2d hex) => _hexModels.ContainsKey(hex.GetHashCode())
			? _hexModels[hex.GetHashCode()]
			: null;
		
		
		public bool TryGetHexModel(Hex2d hex, out HexModel hexModel)
		{
			if (_hexModels.ContainsKey(hex.GetHashCode()))
			{
				hexModel = _hexModels[hex.GetHashCode()];
				return true;
			}
			else
			{
				hexModel = null;
				return false;
			}
		}

		//#456965 похожая логика
		public HexModel CreateHex(Hex2d hexPosition, List<(string, string)> parameters)
		{
			var hexModel = new HexModel(hexPosition, 0, parameters);
			_hexModels.Add(hexModel.GetHashCode(), hexModel);

			Vector3 spawnPosition = _layout.ToPlane(hexPosition);
			HexObject hexInstance = _hexObjectFabric.Create(hexModel, spawnPosition);
			_hexObjects.Add(hexModel.GetHashCode(), hexInstance);

			return hexModel;
		}

		//#456965 похожая логика
		public void CreateHex(HexModel hexModel)
		{
			var copiedHexModel = new HexModel(hexModel);
			_hexModels.Add(copiedHexModel.GetHashCode(), copiedHexModel);

			Vector3 spawnPosition = _layout.ToPlane(hexModel.Q, hexModel.R, hexModel.Height);
			HexObject hexObjectInstance = _hexObjectFabric.Create(copiedHexModel, spawnPosition);
			_hexObjects.Add(copiedHexModel.GetHashCode(), hexObjectInstance);
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

		private void RemoveHexFromHexGrid(HexModel hexModel)
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

		public bool HasHex(Hex2d hex)
		{
			return _hexModels.ContainsKey(hex.GetHashCode());
		}

		//#702659 дублирование
		public bool GetHexIsBlocker(Hex2d hex)
		{
			if (!_hexModels[hex.GetHashCode()].Data
				    .TryGetValue(HexParamsNameConstants.IsBlockerParam, out var isBlocker))
			{
				return false;
			}

			return bool.Parse(isBlocker);
		}

		//#702658 дублирование
		public bool GetHexIsRangeAttackBlocker(Hex2d hex)
		{
			if (!_hexModels[hex.GetHashCode()].Data
				    .TryGetValue(HexParamsNameConstants.IsRangeAttackBlockerParam, out var isBlocker))
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

		public void SetHexIsRangeAttackBlocker(Hex2d hex, bool isBlocker)
		{
			int hexHashCode = hex.GetHashCode();
			
			if (!_hexModels[hexHashCode].Data.ContainsKey(HexParamsNameConstants.IsRangeAttackBlockerParam))
			{
				_hexModels[hexHashCode].Data.Add(HexParamsNameConstants.IsRangeAttackBlockerParam,isBlocker.ToString());
			}
			else
			{
				_hexModels[hexHashCode].Data[HexParamsNameConstants.IsRangeAttackBlockerParam] = isBlocker.ToString();
			}

			_hexObjects[hexHashCode].SetIsRangeAttackBlocker(isBlocker);
		}
	}
}