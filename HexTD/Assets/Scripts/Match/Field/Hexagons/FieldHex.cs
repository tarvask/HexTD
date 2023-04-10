using HexSystem;

namespace Match.Field
{
    public class FieldHex
    {
        public HexModel HexModel { get; }
        public FieldHexType HexType { get; }

        public FieldHex(HexModel hexModel, FieldHexType hexType)
        {
            HexModel = hexModel;
            HexType = hexType;
        }
    }
}