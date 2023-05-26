using System;
using System.Collections.Generic;
using Configs;
using Configs.Constants;
using HexSystem;

namespace MapEditor
{
	public class PropsSpawnerController
	{
		private readonly EditorPropsModel _editorPropsModel;
		private readonly PropsPrefabConfig _propsPrefabConfig;
		private readonly EditorHexesModel _editorHexesModel;
		private readonly HexSpawnerController _hexSpawnerController;
		private string _currentPropsTypeName;

		public PropsSpawnerController(
			EditorPropsModel editorPropsModel,
			PropsPrefabConfig propsPrefabConfig,
			EditorHexesModel editorHexesModel,
			HexSpawnerController hexSpawnerController)
		{
			_editorPropsModel = editorPropsModel;
			_propsPrefabConfig = propsPrefabConfig;
			_editorHexesModel = editorHexesModel;
			_hexSpawnerController = hexSpawnerController;
		}

		public void CreateProps(Hex2d position)
		{
			if (_currentPropsTypeName == null) throw new Exception();

			List<(string, string)> parameters = new List<(string, string)>()
			{
				(PropsParamsNameConstants.Type, _currentPropsTypeName),
				(PropsParamsNameConstants.Rotation, "0"),
			};


			if (!_propsPrefabConfig.PropsObjectConfigs.TryGetValue(_currentPropsTypeName, out var config))
			{
				throw new ArgumentException();
			}

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

		public void SetPropsType(string propsType)
		{
			_currentPropsTypeName = propsType;
		}
	}
}