using HexSystem;

namespace MapEditor.CustomHex
{
    public abstract class BaseHexSetController
    {
        protected readonly Layout Layout;
        protected readonly EditorHexesModel EditorHexesModel;

        public BaseHexSetController(EditorHexesModel editorHexesModel,
            Layout layout)
        {
            EditorHexesModel = editorHexesModel;
            Layout = layout;
        }

        public abstract void HandleInteractWithHex(HexModel hexModel);
        public abstract void HandleRevokeInteractWithHex(HexModel hexModel);
    }
}