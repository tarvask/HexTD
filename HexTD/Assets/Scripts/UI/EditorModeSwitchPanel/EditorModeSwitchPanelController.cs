using Configs;
using InputSystem;
using MapEditor;
using UI.Tools;
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
			foreach (var hexObjectsKey in _hexagonPrefabConfig.HexObjects.Keys)
			{
				var toggleItem = _view.HexTypeToggleGroup.AddToggle(hexObjectsKey);
				firstHexToggle = firstHexToggle == null ? toggleItem.Toggle : firstHexToggle;

				//#623705 дублирование
				AddDisposable(toggleItem.Toggle.OnValueChangedAsObservable()
					.Skip(1)
					.Where(b => b)
					.AsUnitObservable()
					.Subscribe(unit =>
					{
						_view.HexTypeToggleGroup.EnableOneToggle(toggleItem.Toggle,false);
					}));

				AddDisposable(toggleItem.Toggle.OnValueChangedAsObservable()
					.Where(b => b)
					.AsUnitObservable()
					.Subscribe(unit =>
					{
						_hexSpawnerController.SetHexType(hexObjectsKey);
					}));
			}
			_view.HexTypeToggleGroup.EnableOneToggle(firstHexToggle);

			
			Toggle firstPropsToggle = null;
			foreach (var propsObjectsKey in _propsPrefabConfig.PropsObjectConfigs.Keys)
			{
				var toggleItem = _view.PropsTypeToggleGroup.AddToggle(propsObjectsKey);
				firstPropsToggle = firstPropsToggle == null ? toggleItem.Toggle : firstPropsToggle;

				//#623705 дублирование
				AddDisposable(toggleItem.Toggle.OnValueChangedAsObservable()
					.Skip(1)
					.Where(b => b)
					.AsUnitObservable()
					.Subscribe(unit =>
					{
						_view.PropsTypeToggleGroup.EnableOneToggle(toggleItem.Toggle,false);
					}));

				AddDisposable(toggleItem.Toggle.OnValueChangedAsObservable()
					.Where(b => b)
					.AsUnitObservable()
					.Subscribe(unit =>
					{
						_propsSpawnerController.SetPropsType(propsObjectsKey);
					}));
			}
			_view.PropsTypeToggleGroup.EnableOneToggle(firstPropsToggle);

			OnHexModeChosen(true);

			// set default mode
			_view.HexModeToggle.isOn = true;
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