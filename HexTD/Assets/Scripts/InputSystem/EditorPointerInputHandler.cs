using HexSystem;
using MapEditor;
using UnityEngine;

namespace InputSystem
{
    public class EditorPointerInputHandler : IPointerInputListener
    {
        public enum EditMode
        {
            Undefined = 0,
            
            HexMapEdit = 1,
            PathEdit = 2
        }

        private const EditMode DefaultEditMode = EditMode.HexMapEdit;
        
        private readonly HexMapEditorController _hexMapController;
        private readonly PathEditorController _pathEditorController;

        private EditMode _currentEditMode;
        private IPointerInputListener _currentPointerInputListener;

        public EditorPointerInputHandler(HexMapEditorController hexMapController, 
            PathEditorController pathEditorController)
        {
            _hexMapController = hexMapController;
            _pathEditorController = pathEditorController;

            SwitchEditMode(DefaultEditMode);
        }

        private void SwitchEditMode(EditMode editMode = EditMode.Undefined)
        {
            if (editMode == EditMode.Undefined)
                editMode = GetNextEditMode(_currentEditMode);
            
            _currentEditMode = editMode;
            UpdateEditMode();
        }

        private EditMode GetNextEditMode(EditMode editMode)
        {
            switch (editMode)
            {
                case EditMode.HexMapEdit:
                    return EditMode.PathEdit;
                case EditMode.PathEdit:
                    return EditMode.HexMapEdit;
            }

            return EditMode.Undefined;
        }

        private void UpdateEditMode()
        {
            switch (_currentEditMode)
            {
                case EditMode.HexMapEdit:
                    _currentPointerInputListener = _hexMapController;
                    break;
                case EditMode.PathEdit:
                    _currentPointerInputListener = _pathEditorController;
                    break;
                
                default:
                    Debug.LogError("Undefined edit mode set!");
                    break;
            }
        }
        
        public void LmbClickHandle(Hex2d hex)
        {
            _currentPointerInputListener.LmbClickHandle(hex);
        }

        public void RmbClickHandle(Hex2d hex)
        {
            _currentPointerInputListener.RmbClickHandle(hex);
        }

        public void KeyboardInputHandle(Hex2d hex, bool isHexUnderMouse)
        {
            _hexMapController.ApplyKeyboardInput(hex, isHexUnderMouse);
            
            if(Input.GetKeyDown(KeyCode.Space))
                SwitchEditMode();
        }
    }
}