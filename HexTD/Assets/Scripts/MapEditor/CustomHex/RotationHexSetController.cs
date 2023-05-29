using Configs.Constants;
using HexSystem;
using UnityEngine;

namespace MapEditor.CustomHex
{
    public class RotationHexSetController : BaseHexSetController
    {
        public RotationHexSetController(EditorHexesModel editorHexesModel, 
            Layout layout) : base(editorHexesModel, layout)
        {
        }
        
        public override void HandleInteractWithHex(HexModel hexModel)
        {
            UpdateHexObjectRotation(hexModel, 1);
        }

        public override void HandleRevokeInteractWithHex(HexModel hexModel)
        {
            UpdateHexObjectRotation(hexModel, -1);
        }

        private void UpdateHexObjectRotation(HexModel hexModel, int directionSign)
        {
            int stepNum = 0;
            if (!hexModel.Data.TryGetValue(HexParamsNameConstants.HexRotationParam, out string rotation))
            {
                hexModel.Data.Add(HexParamsNameConstants.HexRotationParam, stepNum.ToString());
            }
            else
            {
                stepNum = int.Parse(rotation) + directionSign;
                hexModel.Data[HexParamsNameConstants.HexRotationParam] = (stepNum % 6).ToString();
            }
            
            HexObject bridgeInstance = EditorHexesModel.GetHexagonInstance(hexModel);
            bridgeInstance.transform.Rotate(Vector3.up, MapConstants.AngleStep * directionSign);
        }
    }
}