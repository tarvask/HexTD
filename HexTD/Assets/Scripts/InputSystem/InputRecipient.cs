using System.Collections.Generic;
using HexSystem;
using Lean.Touch;
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

        private bool _isHexUnderMouse;
        private Hex2d _hexUnderMouse;
        
        public InputRecipient(EditorCameraMovementService editorCameraMovementService,
            EditorPointerInputHandler editorPointerInputHandler,
            EditorHexInteractService editorHexInteractService,
            LevelEditorSaveController levelEditorSaveController)
        {
            _editorCameraMovementService = editorCameraMovementService;
            _editorPointerInputHandler = editorPointerInputHandler;
            _editorHexInteractService = editorHexInteractService;
            _levelEditorSaveController = levelEditorSaveController;

            _isHexUnderMouse = false;
            LeanTouch.OnFingerDown += OnMouseClick;
            LeanTouch.OnGesture += OnGesture;
        }

        public void ApplyInput(float frameLength)
        {
            _editorCameraMovementService.ProcessPossibleCameraMovementInput(frameLength);
            _editorPointerInputHandler.KeyboardInputHandle(_hexUnderMouse, _isHexUnderMouse);
            
            if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl)) 
                _levelEditorSaveController.Save();

            if (Input.GetKeyDown(KeyCode.L) && Input.GetKey(KeyCode.LeftControl)) 
                _levelEditorSaveController.Load();
        }

        private void OnGesture(List<LeanFinger> fingers)
        {
            var lastFinger = fingers[^1];
            _isHexUnderMouse = _editorHexInteractService.TryGetHexUnderPointer(lastFinger, out _hexUnderMouse);
        }

        private void OnMouseClick(LeanFinger finger)
        {
            if(!_isHexUnderMouse)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                _editorPointerInputHandler.LmbClickHandle(_hexUnderMouse);
            }

            if (Input.GetMouseButtonDown(1))
            {
                _editorPointerInputHandler.RmbClickHandle(_hexUnderMouse);
            }
        }
    }
}