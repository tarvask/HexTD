using HexSystem;
using InputSystem;
using MapEditor.CustomHex;
using UnityEngine;

namespace MapEditor
{
    public class HexMapEditorController : IPointerInputListener
    {
        public enum EditingHexMode
        {
            Undefined = 0,
            
            HexHeightEdit = 1,
            HexRotationEdit = 2
        }

        private const EditingHexMode DefaultHexType = EditingHexMode.HexHeightEdit;
        
        private readonly HexGridModel _hexGridModel;
        private readonly HexSpawnerController _hexSpawnerController;
        
        private readonly HeightHexSetController _heightHexSetController;
        private readonly RotationHexSetController _rotationHexSetController;

        private EditingHexMode _currentHexEditorMode;
        private BaseHexSetController _currentHexSetController;

        public HexMapEditorController(HexGridModel hexGridModel,
            HexSpawnerController hexSpawnerController,
            HeightHexSetController heightHexSetController,
            RotationHexSetController rotationHexSetController)
        {
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
            HexModel hexModel = _hexGridModel.GetData(hex2d);
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
            HexModel hexModel = _hexGridModel.GetData(hex2d);
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
            
            if (Input.GetKeyDown(KeyCode.P) && Input.GetKey(KeyCode.LeftControl)) 
                _hexGridModel.Clear();
        }
    }
}