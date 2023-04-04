using HexSystem;

namespace MapEditor.CustomHex
{
    public abstract class BaseHexSetController
    {
        protected readonly Layout Layout;
        protected readonly HexGridModel HexGridModel;

        public BaseHexSetController(HexGridModel hexGridModel,
            Layout layout)
        {
            HexGridModel = hexGridModel;
            Layout = layout;
        }

        public abstract void HandleInteractWithHex(HexModel hexModel);
        public abstract void HandleRevokeInteractWithHex(HexModel hexModel);
    }
}