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
            PropsEdit = 2,
            PathEdit = 3
        }

        private const EditMode DefaultEditMode = EditMode.HexMapEdit;
        
        private readonly EditorHexesController _editorHexesController;
        private readonly EditorPropsController _editorPropsController;
        private readonly PathEditorController _pathEditorController;

        private EditMode _currentEditMode;
        private IPointerInputListener _currentPointerInputListener;

        public EditorPointerInputHandler(
            EditorHexesController editorHexesController, 
            EditorPropsController editorPropsController, 
            PathEditorController pathEditorController)
        {
            _editorHexesController = editorHexesController;
            _editorPropsController = editorPropsController;
            _pathEditorController = pathEditorController;

            SwitchEditMode(DefaultEditMode);
        }

        public void SwitchEditMode(EditMode editMode = EditMode.Undefined)
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
                    _currentPointerInputListener = _editorHexesController;
                    break;
                case EditMode.PropsEdit:
                    _currentPointerInputListener = _editorPropsController;
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
            _editorHexesController.ApplyKeyboardInput(hex, isHexUnderMouse);
            _editorPropsController.ApplyKeyboardInput(hex, isHexUnderMouse);
        }
    }
}