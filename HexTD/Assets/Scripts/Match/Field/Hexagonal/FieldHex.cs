using HexSystem;

namespace Match.Field.Hexagonal
{
    public class FieldHex
    {
        private readonly HexModel _hexModel;
        private FieldHexType _fieldHexType;

        public Hex2d Position => _hexModel.Position;
        public int Q => _hexModel.Position.Q;
        public int R => _hexModel.Position.R;
        public int H => _hexModel.Height;

        public string HexObjectTypeName => _hexModel.HexType;
        public FieldHexType FieldHexType => _fieldHexType;

        public FieldHex(HexModel hexModel, FieldHexType fieldHexType)
        {
            _hexModel = hexModel;
            _fieldHexType = fieldHexType;
        }

        public bool TryAddTower()
        {
            if (_fieldHexType != FieldHexType.Free)
                return false;
            
            _fieldHexType = FieldHexType.Tower;
            return true;
        }

        public bool TryRemoveTower()
        {
            if (_fieldHexType != FieldHexType.Tower)
                return false;
            
            _fieldHexType = FieldHexType.Free;
            return true;
        }

        public override int GetHashCode()
        {
            return Q << 8 | R;
        }
    }
}