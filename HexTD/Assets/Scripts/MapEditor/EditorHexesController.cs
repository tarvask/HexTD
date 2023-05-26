using HexSystem;
using InputSystem;
using MapEditor.CustomHex;
using UnityEngine;
using Zenject;

namespace MapEditor
{
    public class EditorHexesController : IPointerInputListener
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
        
        private readonly EditorHexesModel _editorHexesModel;
        private readonly HexSpawnerController _hexSpawnerController;
        
        private readonly HeightHexSetController _heightHexSetController;
        private readonly RotationHexSetController _rotationHexSetController;

        private EditingHexMode _currentHexEditorMode;
        private BaseHexSetController _currentHexSetController;

        public EditorHexesController(
            [Inject(Id = KeyEditableAreaSize)] Hex2d areaSize,
            EditorHexesModel editorHexesModel,
            HexSpawnerController hexSpawnerController,
            HeightHexSetController heightHexSetController,
            RotationHexSetController rotationHexSetController)
        {
            _areaSize = areaSize;
            _areaMin = new Hex2d(0, 0);
            _areaMax = _areaMin + _areaSize;
            
            _editorHexesModel = editorHexesModel;
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
            if (!IsHexInHexRect(hex2d, _areaMin, _areaMax))
            {
                return;
            }

            HexModel hexModel = _editorHexesModel.GetHexModel(hex2d);
            if (hexModel == null)
            {
                _hexSpawnerController.CreateHex(hex2d);
            }
            else
            {
                _currentHexSetController.HandleInteractWithHex(hexModel);
            }
        }

        public void RmbClickHandle(Hex2d hex2d)
        {
            HexModel hexModel = _editorHexesModel.GetHexModel(hex2d);
            if (hexModel == null)
                return;

            _currentHexSetController.HandleRevokeInteractWithHex(hexModel);
        }

        public void ApplyKeyboardInput(Hex2d hex, bool isHexUnderMouse)
        {
            _hexSpawnerController.UpdateHexType();
            
            if (Input.GetKeyDown(KeyCode.R))
                SwitchHexSetController();

            if (isHexUnderMouse && !Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.X))
            {
                _editorHexesModel.RemoveHexFromHexGrid(hex);
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                if (_editorHexesModel.HasHex(hex))
                {
                    var isHexBlocker = _editorHexesModel.GetHexIsBlocker(hex);
                    _editorHexesModel.SetHexIsBlocker(hex, !isHexBlocker);
                }
            }
            
            if (Input.GetKeyDown(KeyCode.N))
            {
                if (_editorHexesModel.HasHex(hex))
                {
                    var isHexBlocker = _editorHexesModel.GetHexIsRangeAttackBlocker(hex);
                    _editorHexesModel.SetHexIsRangeAttackBlocker(hex, !isHexBlocker);
                }
            }
            
            if (Input.GetKeyDown(KeyCode.P) && Input.GetKey(KeyCode.LeftControl)) 
                _editorHexesModel.Clear();
        }
        
        private static bool IsHexInHexRect(Hex2d hex, Hex2d min, Hex2d max)
        {
            Hex2d size = max - min;
            Hex2d point = hex - min;

            if (point.Q >= -Mathf.FloorToInt(point.R * 0.5f)
                && point.Q < size.Q - Mathf.FloorToInt(point.R * 0.5f)
                && point.R >= 0
                && point.R < size.R)
            {
                return true;
            }

            return false;
        }
    }
}