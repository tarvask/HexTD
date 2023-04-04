using System;
using System.Collections.Generic;
using HexSystem;
using InputSystem;
using Object = UnityEngine.Object;

namespace MapEditor
{
	public class HexMapController : IPointerInputListener
	{
		public ICollection<HexModel> HexModels => _hexModels.Values;

		private readonly HexObject _hexPrefab;

		private IDictionary<Hex2d, HexModel> _hexModels = new Dictionary<Hex2d, HexModel>();
		private readonly IDictionary<Hex2d, HexObject> _hexObjects;

		private readonly Layout _layout;

		public HexMapController(Layout layout, HexObject hexPrefab)
		{
			_layout = layout;
			_hexPrefab = hexPrefab;

			_hexObjects = new Dictionary<Hex2d, HexObject>(_hexModels.Count);

			IncreaseHex(new Hex2d(0, 0));
			IncreaseHex(new Hex2d(1, 0));
			IncreaseHex(new Hex2d(0, 1));
			IncreaseHex(new Hex2d(0, 2));
		}

		public void LmbClickHandle(Hex2d hex) => IncreaseHex(hex);

		public void RmbClickHandle(Hex2d hex) => DecreaseHex(hex);

		public void IncreaseHex(Hex2d hex)
		{
			Hex3d hex3d;
			HexObject hexObject;
			if (_hexModels.ContainsKey(hex))
			{
				_hexModels[hex].Height++;
				hex3d = (Hex3d)_hexModels[hex];

				hexObject = _hexObjects[hex];
			}
			else
			{
				_hexModels.Add(hex, new HexModel( hex, 0));
				hex3d = new Hex3d(hex, 0);

				hexObject = CreateNewHexObject(hex);
			}

			hexObject.transform.position = _layout.ToPlane(hex3d);
		}

		public void DecreaseHex(Hex2d hex)
		{
			if (!_hexModels.ContainsKey(hex))
			{
				return;
			}

			_hexModels[hex].Height--;

			if (_hexModels[hex].Height >= 0)
			{
				_hexObjects[hex].transform.position = _layout.ToPlane((Hex3d)_hexModels[hex]);
			}
			else
			{
				RemoveHex(hex);
			}
		}

		public void SetHex(HexModel hexModel)
		{
			HexObject hexObject;
			if (!_hexModels.ContainsKey(hexModel.Position))
			{
				_hexModels.Add(hexModel.Position, hexModel);
				hexObject = CreateNewHexObject(hexModel.Position);
			}
			else
			{
				_hexModels[hexModel.Position] = hexModel;
				hexObject = _hexObjects[hexModel.Position];
			}

			hexObject.transform.position = _layout.ToPlane((Hex3d)hexModel);
		}

		public void RemoveHex(Hex2d hex)
		{
			if (!_hexModels.ContainsKey(hex))
			{
				return;
			}

			Object.Destroy(_hexObjects[hex].gameObject);
			_hexObjects.Remove(hex);

			_hexModels.Remove(hex);
		}

		public void ClearAll()
		{
			foreach (var hexObject in _hexObjects)
			{
				Object.Destroy(hexObject.Value.gameObject);
			}

			_hexObjects.Clear();

			_hexModels.Clear();
		}

		private HexObject CreateNewHexObject(Hex2d position)
		{
			if (_hexObjects.ContainsKey(position))
			{
				throw new ArgumentException("Hex object already exist");
			}

			HexObject hexObject = Object.Instantiate(_hexPrefab);
			hexObject.name = position.ToString();
			_hexObjects.Add(position, hexObject);

			return hexObject;
		}
	}
}