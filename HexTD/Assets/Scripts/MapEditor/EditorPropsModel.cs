using System.Collections.Generic;
using HexSystem;
using UnityEngine;

namespace MapEditor
{
	public class EditorPropsModel
	{
		private readonly EditorPropsObjectFabric _propsObjectFabric;
		private readonly Layout _layout;
		private readonly IDictionary<int, PropsModel> _propsModels;
		private readonly IDictionary<int, PropsObject> _propsObjects;

		public EditorPropsModel(EditorPropsObjectFabric propsObjectFabric, Layout layout)
		{
			_propsObjectFabric = propsObjectFabric;
			_layout = layout;
			_propsModels = new Dictionary<int, PropsModel>();
			_propsObjects = new Dictionary<int, PropsObject>();
		}

		public PropsModel GetPropsModel(Hex2d hex) => _propsModels.ContainsKey(hex.GetHashCode())
			? _propsModels[hex.GetHashCode()]
			: null;

		//#456965 похожая логика
		public PropsModel CreateProps(Hex2d position, int height, List<(string, string)> parameters)
		{
			var model = new PropsModel(position, height, parameters);
			_propsModels.Add(model.GetHashCode(), model);

			Vector3 spawnPosition = _layout.ToPlane((Hex3d)model);
			var propsObject = _propsObjectFabric.Create(model, spawnPosition);
			_propsObjects.Add(model.GetHashCode(), propsObject);

			return model;
		}

		//#456965 похожая логика
		public void CreateProps(PropsModel model)
		{
			var copiedModel = new PropsModel(model);
			_propsModels.Add(copiedModel.GetHashCode(), copiedModel);

			Vector3 spawnPosition = _layout.ToPlane(model.Q, model.R, model.Height);
			var propsObject = _propsObjectFabric.Create(copiedModel, spawnPosition);
			_propsObjects.Add(copiedModel.GetHashCode(), propsObject);
		}

		public PropsObject GetPropsObject(PropsModel hexModel)
		{
			return _propsObjects[hexModel.GetHashCode()];
		}

		public void RemovePropsFromGrid(Hex2d hex)
		{
			var model = GetPropsModel(hex);
			if (model == null)
				return;

			RemovePropsFromGrid(model);
		}

		private void RemovePropsFromGrid(PropsModel model)
		{
			if (!_propsObjects.Remove(model.GetHashCode(), out var propsObject))
				return;

			Object.Destroy(propsObject.gameObject);
			_propsModels.Remove(model.GetHashCode());
		}

		public List<PropsModel> GetAllProps()
		{
			return new List<PropsModel>(_propsModels.Values);
		}

		public void Clear()
		{
			foreach (var propsObject in _propsObjects)
			{
				Object.Destroy(propsObject.Value.gameObject);
			}

			_propsObjects.Clear();
			_propsModels.Clear();
		}

		public bool IsHasModel(Hex2d hex)
		{
			return _propsModels.ContainsKey(hex.GetHashCode());
		}
	}
}