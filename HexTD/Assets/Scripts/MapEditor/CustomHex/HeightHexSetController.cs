using Configs.Constants;
using HexSystem;

namespace MapEditor.CustomHex
{
    public class HeightHexSetController : BaseHexSetController
    {
        public HeightHexSetController(HexGridModel hexGridModel, 
            Layout layout) : base(hexGridModel, layout)
        {
        }

        public override void HandleInteractWithHex(HexModel hexModel)
        {
            ++hexModel.Height;
            UpdateHexObjectPosition(hexModel);
        }

        public override void HandleRevokeInteractWithHex(HexModel hexModel)
        {
            if (hexModel.Height > 0)
            {
                --hexModel.Height;
                UpdateHexObjectPosition(hexModel);
            }
        }

        private void UpdateHexObjectPosition(HexModel hexModel)
        {
            HexGridModel.GetHexagonInstance(hexModel).transform.position = Layout.ToPlane((Hex3d)hexModel);
        }
    }
}