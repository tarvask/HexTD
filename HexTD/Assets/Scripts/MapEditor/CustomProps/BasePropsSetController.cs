using HexSystem;

namespace MapEditor.CustomProps
{
	public abstract class BasePropsSetController
	{
		protected readonly Layout Layout;
		protected readonly EditorPropsModel EditorPropsModel;

		public BasePropsSetController(EditorPropsModel editorPropsModel,
			Layout layout)
		{
			EditorPropsModel = editorPropsModel;
			Layout = layout;
		}

		public abstract void HandleInteractWithProps(PropsModel propsModel);
		public abstract void HandleRevokeInteractWithProps(PropsModel propsModel);
	}
}