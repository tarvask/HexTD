using Configs.Constants;
using HexSystem;
using UnityEngine;

namespace MapEditor.CustomProps
{
	public class RotationPropsSetController: BasePropsSetController
	{
		public RotationPropsSetController(EditorPropsModel editorPropsModel, 
			Layout layout) : base(editorPropsModel, layout)
		{
		}
        
		public override void HandleInteractWithProps(PropsModel propsModel)
		{
			UpdatePropsObjectRotation(propsModel, 1);
		}

		public override void HandleRevokeInteractWithProps(PropsModel propsModel)
		{
			UpdatePropsObjectRotation(propsModel, -1);
		}

		private void UpdatePropsObjectRotation(PropsModel propsModel, int directionSign)
		{
			int stepNum = 0;
			if (!propsModel.Data.TryGetValue(PropsParamsNameConstants.Rotation, out string rotation))
			{
				propsModel.Data.Add(PropsParamsNameConstants.Rotation, stepNum.ToString());
			}
			else
			{
				stepNum = int.Parse(rotation) + directionSign;
				propsModel.Data[PropsParamsNameConstants.Rotation] = (stepNum % 6).ToString();
			}
            
			PropsObject instance = EditorPropsModel.GetPropsObject(propsModel);
			instance.transform.Rotate(Vector3.up, MapConstants.AngleStep * directionSign);
		}
	}
}