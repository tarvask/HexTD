using HexSystem;
using InputSystem;
using MapEditor.CustomProps;
using UnityEngine;
using Zenject;

namespace MapEditor
{
    public class EditorPropsController : IPointerInputListener
    {
        private enum EditingMode
        {
            Undefined = 0,

            HeightEdit = 1,
            RotationEdit = 2
        }

        private const EditingMode DefaultHexType = EditingMode.HeightEdit;

        private readonly Hex2d _areaSize;
        private readonly Hex2d _areaMin;
        private readonly Hex2d _areaMax;

        private readonly EditorPropsModel _editorPropsModel;

        private readonly PropsSpawnerController _propsSpawnerController;

        private readonly HeightPropsSetController _heightPropsSetController;
        private readonly RotationPropsSetController _rotationPropsSetController;

        private EditingMode _currentEditingMode;
        private BasePropsSetController _currentPropsSetController;

        public EditorPropsController(
            [Inject(Id = EditorHexesController.KeyEditableAreaSize)] Hex2d areaSize,
            [Inject(Id = EditorHexesController.KeyEditableAreaMin)] Hex2d areaMin,
            EditorPropsModel editorPropsModel,
            PropsSpawnerController propsSpawnerController,
            HeightPropsSetController heightPropsSetController,
            RotationPropsSetController rotationPropsSetController)
        {
            _areaSize = areaSize;
            _areaMin = areaMin;
            _areaMax = _areaMin + _areaSize;

            _editorPropsModel = editorPropsModel;
            _propsSpawnerController = propsSpawnerController;

            _heightPropsSetController = heightPropsSetController;
            _rotationPropsSetController = rotationPropsSetController;

            _currentEditingMode = DefaultHexType;
            UpdateEditingType();
        }

        public void SwitchPropsSetController()
        {
            _currentEditingMode = GetNextEditingType(_currentEditingMode);
            UpdateEditingType();
        }

        private EditingMode GetNextEditingType(EditingMode editingMode)
        {
            switch (editingMode)
            {
                case EditingMode.HeightEdit:
                    return EditingMode.RotationEdit;
                case EditingMode.RotationEdit:
                    return EditingMode.HeightEdit;
            }

            return EditingMode.Undefined;
        }

        private void UpdateEditingType()
        {
            switch (_currentEditingMode)
            {
                case EditingMode.HeightEdit:
                    _currentPropsSetController = _heightPropsSetController;
                    return;
                case EditingMode.RotationEdit:
                    _currentPropsSetController = _rotationPropsSetController;
                    return;
            }
        }

        public void LmbClickHandle(Hex2d hex2d)
        {
            if (!IsHexInHexRect(hex2d, _areaMin, _areaMax))
            {
                return;
            }

            PropsModel propsModel = _editorPropsModel.GetPropsModel(hex2d);
            if (propsModel == null)
            {
                _propsSpawnerController.CreateObjects(hex2d);
            }
            else
            {
                _currentPropsSetController.HandleInteractWithProps(propsModel);
            }
        }

        public void RmbClickHandle(Hex2d hex2d)
        {
            PropsModel propsModel = _editorPropsModel.GetPropsModel(hex2d);
            if (propsModel == null)
                return;

            _currentPropsSetController.HandleRevokeInteractWithProps(propsModel);
        }

        public void ApplyKeyboardInput(Hex2d hex, bool isHexUnderMouse)
        {
            if (Input.GetKeyDown(KeyCode.R))
                SwitchPropsSetController();

            if (isHexUnderMouse && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.X))
            {
                _editorPropsModel.RemovePropsFromGrid(hex);
            }
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