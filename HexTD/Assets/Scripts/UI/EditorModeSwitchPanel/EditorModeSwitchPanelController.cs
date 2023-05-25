using Configs.Constants;
using InputSystem;
using MapEditor;
using UI.Tools;
using UnityEngine.UI;

namespace UI.EditorModeSwitchPanel
{
    public class EditorModeSwitchPanelController : BaseDisposable
    {
        private readonly EditorModeSwitchPanelView _view;
        private readonly EditorPointerInputHandler _pointerInputHandler;
        private readonly HexSpawnerController _hexSpawnerController;

        public EditorModeSwitchPanelController(EditorModeSwitchPanelView view, 
            EditorPointerInputHandler pointerInputHandler,
            HexSpawnerController hexSpawnerController)
        {
            _view = view;
            _pointerInputHandler = pointerInputHandler;
            _hexSpawnerController = hexSpawnerController;
            
            _view.HexPathModesToggleGroup.RegisterToggle(_view.HexModeToggle);
            _view.HexPathModesToggleGroup.RegisterToggle(_view.PathModeToggle);
            _view.HexModeToggle.onValueChanged.AddListener(OnHexModeChosen);
            _view.PathModeToggle.onValueChanged.AddListener(OnPathModeChosen);

            _view.HexTypeModeToggleGroup.RegisterToggle(_view.NormalHexToggle);
            _view.HexTypeModeToggleGroup.RegisterToggle(_view.BridgeHexToggle);
            _view.HexTypeModeToggleGroup.RegisterToggle(_view.StonePropsHexToggle);
            _view.HexTypeModeToggleGroup.RegisterToggle(_view.BushPropsHexToggle);
            _view.HexTypeModeToggleGroup.RegisterToggle(_view.TreePropsHexToggle);
            _view.HexTypeModeToggleGroup.RegisterToggle(_view.GrassPropsHexToggle);
            _view.HexTypeModeToggleGroup.RegisterToggle(_view.MushroomSinglePropsHexToggle);
            _view.HexTypeModeToggleGroup.RegisterToggle(_view.MushroomClusterPropsHexToggle);
            
            _view.NormalHexToggle.onValueChanged.AddListener((isChosen) =>
                OnHexModeChanged(_view.NormalHexToggle, HexTypeNameConstants.SimpleType, isChosen));
            _view.BridgeHexToggle.onValueChanged.AddListener((isChosen) =>
                OnHexModeChanged(_view.BridgeHexToggle, HexTypeNameConstants.BridgeType, isChosen));
            // props
            _view.StonePropsHexToggle.onValueChanged.AddListener((isChosen) =>
                OnHexModeChanged(_view.StonePropsHexToggle, HexTypeNameConstants.StonePropsType, isChosen));
            _view.BushPropsHexToggle.onValueChanged.AddListener((isChosen) =>
                OnHexModeChanged(_view.BushPropsHexToggle, HexTypeNameConstants.BushPropsType, isChosen));
            _view.TreePropsHexToggle.onValueChanged.AddListener((isChosen) =>
                OnHexModeChanged(_view.TreePropsHexToggle, HexTypeNameConstants.TreePropsType, isChosen));
            _view.GrassPropsHexToggle.onValueChanged.AddListener((isChosen) =>
                OnHexModeChanged(_view.GrassPropsHexToggle, HexTypeNameConstants.GrassPropsType, isChosen));
            _view.MushroomSinglePropsHexToggle.onValueChanged.AddListener((isChosen) =>
                OnHexModeChanged(_view.MushroomSinglePropsHexToggle, HexTypeNameConstants.MushroomSinglePropsType, isChosen));
            _view.MushroomClusterPropsHexToggle.onValueChanged.AddListener((isChosen) =>
                OnHexModeChanged(_view.MushroomClusterPropsHexToggle, HexTypeNameConstants.MushroomClusterPropsType, isChosen));

            // set default mode
            _view.HexModeToggle.isOn = true;
        }

        private void OnHexModeChosen(bool isChosen)
        {
            if (isChosen)
            {
                _view.PathModeToggle.SetIsOnWithoutNotify(false);
                
                _view.HexTypeModeToggleGroup.gameObject.SetActive(true);
                _view.HexTypeModeToggleGroup.SetAllTogglesOff(false);
                
                _pointerInputHandler.SwitchEditMode(EditorPointerInputHandler.EditMode.HexMapEdit);
                
                // set default hex object
                _view.NormalHexToggle.isOn = true;
            }
            else
            {
                _view.HexTypeModeToggleGroup.gameObject.SetActive(false);
            }

            // set default mode
            if (!_view.HexPathModesToggleGroup.AnyTogglesOn())
                _view.HexModeToggle.isOn = true;
        }

        private void OnPathModeChosen(bool isChosen)
        {
            if (isChosen)
            {
                _view.HexModeToggle.SetIsOnWithoutNotify(false);
                _view.HexTypeModeToggleGroup.gameObject.SetActive(false);
                
                _pointerInputHandler.SwitchEditMode(EditorPointerInputHandler.EditMode.PathEdit);
            }
            else
            {
                _view.HexTypeModeToggleGroup.gameObject.SetActive(true);
            }
            
            // set default mode
            if (!_view.HexPathModesToggleGroup.AnyTogglesOn())
                _view.HexModeToggle.isOn = true;
        }

        private void OnHexModeChanged(Toggle toggle, string hexTypeName, bool isChosen)
        {
            if (isChosen)
            {
                _view.HexTypeModeToggleGroup.SetAllTogglesOff(false);
                toggle.SetIsOnWithoutNotify(true);
                _hexSpawnerController.SetHexType(hexTypeName);
            }
            
            // set default hex object
            if (!_view.HexTypeModeToggleGroup.AnyTogglesOn())
                _view.NormalHexToggle.isOn = true;
        }
    }
}