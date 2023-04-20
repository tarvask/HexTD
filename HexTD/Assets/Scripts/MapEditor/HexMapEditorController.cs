using HexSystem;
using InputSystem;
using MapEditor.CustomHex;
using UnityEngine;
using Zenject;

namespace MapEditor
{
    public class HexMapEditorController : IPointerInputListener
    {
        public const string KeyEditableAreaSize = "EditableAreaSize";
        
        public enum EditingHexMode
        {
            Undefined = 0,
            
            HexHeightEdit = 1,
            HexRotationEdit = 2
        }

        private const EditingHexMode DefaultHexType = EditingHexMode.HexHeightEdit;

        private readonly Hex2d _areaSize;
        private readonly Hex2d _areaMin;
        private readonly Hex2d _areaMax;
        
        private readonly HexGridModel _hexGridModel;
        private readonly HexSpawnerController _hexSpawnerController;
        
        private readonly HeightHexSetController _heightHexSetController;
        private readonly RotationHexSetController _rotationHexSetController;

        private EditingHexMode _currentHexEditorMode;
        private BaseHexSetController _currentHexSetController;

        public HexMapEditorController(
            [Inject(Id = KeyEditableAreaSize)] Hex2d areaSize,
            HexGridModel hexGridModel,
            HexSpawnerController hexSpawnerController,
            HeightHexSetController heightHexSetController,
            RotationHexSetController rotationHexSetController)
        {
            _areaSize = areaSize;
            _areaMin = new Hex2d(0, 0);
            _areaMax = _areaMin + _areaSize;
            
            _hexGridModel = hexGridModel;
            _hexSpawnerController = hexSpawnerController;
            
            _heightHexSetController = heightHexSetController;
            _rotationHexSetController = rotationHexSetController;

            _currentHexEditorMode = DefaultHexType;
            UpdateEditingHexType();
        }

        public void SwitchHexSetController()
        {
            _currentHexEditorMode = GetNextEditingHexType(_currentHexEditorMode);
            UpdateEditingHexType();
        }

        private EditingHexMode GetNextEditingHexType(EditingHexMode editingHexMode)
        {
            switch (editingHexMode)
            {
                case EditingHexMode.HexHeightEdit:
                    return EditingHexMode.HexRotationEdit;
                case EditingHexMode.HexRotationEdit:
                    return EditingHexMode.HexHeightEdit;
            }

            return EditingHexMode.Undefined;
        }

        private void UpdateEditingHexType()
        {
            switch (_currentHexEditorMode)
            {
                case EditingHexMode.HexHeightEdit:
                    _currentHexSetController = _heightHexSetController;
                    return;
                case EditingHexMode.HexRotationEdit:
                    _currentHexSetController = _rotationHexSetController;
                    return;
            }
        }
        
        public void LmbClickHandle(Hex2d hex2d)
        {
            if (!OnHexInHexRect(hex2d, _areaMin, _areaMax))
            {
                return;
            }

            HexModel hexModel = _hexGridModel.GetHexModel(hex2d);
            if (hexModel == null)
            {
                _hexSpawnerController.CreateHex(hex2d);
            }
            else
            {
                _currentHexSetController.HandleInteractWithHex(hexModel);
            }
        }

        private bool OnHexInHexRect(Hex2d hex, Hex2d min, Hex2d max)
        {
            Hex2d size = max - min;
            Hex2d point = hex - min;
//            Debug.Log($"{nameof(hex)}: {hex}");
//            Debug.Log($"{nameof(size)}: {size}");
//            Debug.Log($"{nameof(point)}: {point}");

            if (point.Q >= -Mathf.FloorToInt(point.R / 2.0f)
                && point.Q < size.Q - Mathf.FloorToInt(point.R / 2.0f)
                && point.R >= 0
                && point.R < size.R)
            {
                return true;
            }

            return false;
        }

        public void RmbClickHandle(Hex2d hex2d)
        {
            HexModel hexModel = _hexGridModel.GetHexModel(hex2d);
            if (hexModel == null)
                return;

            _currentHexSetController.HandleRevokeInteractWithHex(hexModel);
        }

        public void ApplyKeyboardInput(Hex2d hex, bool isHexUnderMouse)
        {
            _hexSpawnerController.UpdateHexType();
            
            if(Input.GetKeyDown(KeyCode.R))
                SwitchHexSetController();
            
            if(isHexUnderMouse && Input.GetKeyDown(KeyCode.X))
                _hexGridModel.RemoveHexFromHexGrid(hex);

            if (Input.GetKeyDown(KeyCode.B))
            {
                var isHexBlocker = _hexGridModel.GetHexIsBlocker(hex);
                _hexGridModel.SetHexIsBlocker(hex, !isHexBlocker);
            }
            
            if (Input.GetKeyDown(KeyCode.P) && Input.GetKey(KeyCode.LeftControl)) 
                _hexGridModel.Clear();
        }
    }
}