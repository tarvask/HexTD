using System;
using System.Collections.Generic;
using Configs;
using Configs.Constants;
using HexSystem;
using Match.Field;

namespace MapEditor
{
	public class PropsSpawnerController : IObjectsSpawner
	{
		private readonly EditorPropsModel _editorPropsModel;
		private readonly IPropsObjectPrefabConfigRetriever _propsObjectPrefabConfigRetriever;
		private readonly EditorHexesModel _editorHexesModel;
		private readonly HexSpawnerController _hexSpawnerController;
		private string _currentPropsTypeName;

		public PropsSpawnerController(
			EditorPropsModel editorPropsModel,
			IPropsObjectPrefabConfigRetriever propsObjectPrefabConfigRetriever,
			EditorHexesModel editorHexesModel,
			HexSpawnerController hexSpawnerController)
		{
			_editorPropsModel = editorPropsModel;
			_propsObjectPrefabConfigRetriever = propsObjectPrefabConfigRetriever;
			_editorHexesModel = editorHexesModel;
			_hexSpawnerController = hexSpawnerController;
		}

		public void CreateObjects(Hex2d position)
		{
			if (_currentPropsTypeName == null) throw new Exception();

			List<(string, string)> parameters = new List<(string, string)>()
			{
				(PropsParamsNameConstants.Type, _currentPropsTypeName),
				(PropsParamsNameConstants.Rotation, "0"),
			};

			var config = _propsObjectPrefabConfigRetriever.GetPropsByType(_currentPropsTypeName);

			if (config.PropsPlacingConfig.IsReplacesHex)
			{
				var isBlocker = false;
				var isRangeAttackBlocker = false;
				if (_editorHexesModel.TryGetHexModel(position, out var x))
				{
					isBlocker = _editorHexesModel.GetHexIsBlocker(position);
					isRangeAttackBlocker = _editorHexesModel.GetHexIsRangeAttackBlocker(position);
				}

				_editorHexesModel.RemoveHexFromHexGrid(position);

				if (isBlocker || isRangeAttackBlocker)
				{
					if (!_editorHexesModel.HasHex(position))
					{
						_hexSpawnerController.CreateInvisibleHex(position);
					}
				}

				if (isBlocker)
				{
					_editorHexesModel.SetHexIsBlocker(position, true);
				}

				if (isRangeAttackBlocker)
				{
					_editorHexesModel.SetHexIsRangeAttackBlocker(position, true);
				}
			}

			var propsHeight = _editorHexesModel.TryGetHexModel(position, out var hexModel)
				? hexModel.Height
				: 0;

			PropsModel model = _editorPropsModel.CreateProps(position, propsHeight, parameters);

			if (config.PropsPlacingConfig.IsBuildingBlock ||
			    config.PropsPlacingConfig.IsRangeAttackBlock
			   )
			{
				if (!_editorHexesModel.HasHex(position))
				{
					_hexSpawnerController.CreateInvisibleHex(position);
				}
			}

			if (config.PropsPlacingConfig.IsBuildingBlock)
			{
				_editorHexesModel.SetHexIsBlocker(position, true);
			}

			if (config.PropsPlacingConfig.IsRangeAttackBlock)
			{
				_editorHexesModel.SetHexIsRangeAttackBlocker(position, true);
			}
		}

		public void SetObjectType(string propsType)
		{
			_currentPropsTypeName = propsType;
		}
	}
}