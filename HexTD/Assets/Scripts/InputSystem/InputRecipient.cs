using HexSystem;
using MapEditor;
using UnityEngine;

namespace InputSystem
{
    public class InputRecipient
    {
        private readonly EditorCameraMovementService _editorCameraMovementService;
        private readonly EditorPointerInputHandler _editorPointerInputHandler;
        private readonly EditorHexInteractService _editorHexInteractService;
        private readonly LevelEditorSaveController _levelEditorSaveController;

        public InputRecipient(EditorCameraMovementService editorCameraMovementService,
            EditorPointerInputHandler editorPointerInputHandler,
            EditorHexInteractService editorHexInteractService,
            LevelEditorSaveController levelEditorSaveController)
        {
            _editorCameraMovementService = editorCameraMovementService;
            _editorPointerInputHandler = editorPointerInputHandler;
            _editorHexInteractService = editorHexInteractService;
            _levelEditorSaveController = levelEditorSaveController;
        }

        public void ApplyInput(float frameLength)
        {
            bool isHit = _editorHexInteractService.TryGetHexUnderPointer(out var hitPoint);
            
            _editorCameraMovementService.ProcessPossibleCameraMovementInput(frameLength);
            _editorPointerInputHandler.KeyboardInputHandle(hitPoint);
            
            if (Input.GetMouseButtonDown(0))
            {
                if (isHit)
                {
                    _editorPointerInputHandler.LmbClickHandle(hitPoint);
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (isHit)
                {
                    _editorPointerInputHandler.RmbClickHandle(hitPoint);
                }
            }

            if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl)) 
                _levelEditorSaveController.Save();

            if (Input.GetKeyDown(KeyCode.L) && Input.GetKey(KeyCode.LeftControl)) 
                _levelEditorSaveController.Load();
        }
    }
}