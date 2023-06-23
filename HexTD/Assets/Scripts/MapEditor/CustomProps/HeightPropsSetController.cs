using HexSystem;

namespace MapEditor.CustomProps
{
	public class HeightPropsSetController: BasePropsSetController
	{
		public HeightPropsSetController(EditorPropsModel editorPropsModel, 
			Layout layout) : base(editorPropsModel, layout)
		{
		}

		public override void HandleInteractWithProps(PropsModel propsModel)
		{
			++propsModel.Height;
			UpdatePropsObjectPosition(propsModel);
		}

		public override void HandleRevokeInteractWithProps(PropsModel propsModel)
		{
			if (propsModel.Height > 0)
			{
				--propsModel.Height;
				UpdatePropsObjectPosition(propsModel);
			}
		}

		private void UpdatePropsObjectPosition(PropsModel propsModel)
		{
			EditorPropsModel.GetPropsObject(propsModel).transform.position = Layout.ToPlane((Hex3d)propsModel);
		}
	}
}