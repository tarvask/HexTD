using System.Collections.Generic;
using Configs;
using InputSystem;
using MapEditor;
using UI.Tools;
using UI.Tools.SimpleToggle;
using UniRx;
using UnityEngine.UI;

namespace UI.EditorModeSwitchPanel
{
	public class EditorModeSwitchPanelController : BaseDisposable
	{
		private readonly EditorModeSwitchPanelView _view;
		private readonly EditorPointerInputHandler _pointerInputHandler;
		private readonly HexSpawnerController _hexSpawnerController;
		private readonly PropsSpawnerController _propsSpawnerController;
		private readonly HexagonPrefabConfig _hexagonPrefabConfig;
		private readonly PropsPrefabConfig _propsPrefabConfig;

		public EditorModeSwitchPanelController(EditorModeSwitchPanelView view,
			EditorPointerInputHandler pointerInputHandler,
			HexSpawnerController hexSpawnerController,
			PropsSpawnerController propsSpawnerController,
			HexagonPrefabConfig hexagonPrefabConfig,
			PropsPrefabConfig propsPrefabConfig)
		{
			_view = view;
			_pointerInputHandler = pointerInputHandler;
			_hexSpawnerController = hexSpawnerController;
			_propsSpawnerController = propsSpawnerController;
			_hexagonPrefabConfig = hexagonPrefabConfig;
			_propsPrefabConfig = propsPrefabConfig;

			_view.HexModeToggle.onValueChanged.AddListener(OnHexModeChosen);
			_view.PropsModeToggle.onValueChanged.AddListener(OnPropsModeChosen);
			_view.PathModeToggle.onValueChanged.AddListener(OnPathModeChosen);

			Toggle firstHexToggle = null;
			InitTogglesGroup(_hexSpawnerController,
				_hexagonPrefabConfig.HexObjects.Keys,
				_view.HexTypeToggleGroup,
				ref firstHexToggle);
			
			Toggle firstPropsToggle = null;
			InitTogglesGroup(_propsSpawnerController,
				_propsPrefabConfig.PropsObjectConfigs.Keys,
				_view.PropsTypeToggleGroup,
				ref firstPropsToggle);

			OnHexModeChosen(true);

			// set default mode
			_view.HexModeToggle.isOn = true;
		}

		private void InitTogglesGroup(
			IObjectsSpawner objectsSpawner,
			IEnumerable<string> keys,
			SimpleToggleGroup simpleToggleGroup,
			ref Toggle firstToggleInGroup)
		{
			foreach (var objectKey in keys)
			{
				var simpleToggleItem = simpleToggleGroup.AddToggle(objectKey);
				firstToggleInGroup = firstToggleInGroup == null ? simpleToggleItem.Toggle : firstToggleInGroup;
				
				AddDisposable(simpleToggleItem.Toggle.OnValueChangedAsObservable()
					.Skip(1)
					.Where(b => b)
					.AsUnitObservable()
					.Subscribe(unit => { simpleToggleGroup.EnableOneToggle(simpleToggleItem.Toggle, false); }));

				AddDisposable(simpleToggleItem.Toggle.OnValueChangedAsObservable()
					.Where(b => b)
					.AsUnitObservable()
					.Subscribe(unit => { objectsSpawner.SetObjectType(objectKey); }));
			}
			
			simpleToggleGroup.EnableOneToggle(firstToggleInGroup);
		}

		private void OnHexModeChosen(bool isChosen)
		{
			if (isChosen)
			{
				_view.HexTypeScrollRect.gameObject.SetActive(true);
				_view.PropsTypeScrollRect.gameObject.SetActive(false);

				_pointerInputHandler.SwitchEditMode(EditorPointerInputHandler.EditMode.HexMapEdit);
			}
		}

		private void OnPropsModeChosen(bool isChosen)
		{
			if (isChosen)
			{
				_view.HexTypeScrollRect.gameObject.SetActive(false);
				_view.PropsTypeScrollRect.gameObject.SetActive(true);

				_pointerInputHandler.SwitchEditMode(EditorPointerInputHandler.EditMode.PropsEdit);
			}
		}

		private void OnPathModeChosen(bool isChosen)
		{
			if (isChosen)
			{
				_view.HexTypeScrollRect.gameObject.SetActive(false);
				_view.PropsTypeScrollRect.gameObject.SetActive(false);

				_pointerInputHandler.SwitchEditMode(EditorPointerInputHandler.EditMode.PathEdit);
			}
		}
	}
}