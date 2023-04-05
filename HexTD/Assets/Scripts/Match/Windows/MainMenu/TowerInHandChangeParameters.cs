using Match.Field.Tower;

namespace Match.Windows.MainMenu
{
    public struct TowerInHandChangeParameters
    {
        private readonly byte _handNumber;
        private readonly byte _slotNumber;
        private readonly TowerType _towerType;

        public byte HandNumber => _handNumber;
        public byte SlotNumber => _slotNumber;
        public TowerType TowerType => _towerType;

        public TowerInHandChangeParameters(byte handNumber, byte slotNumber, TowerType towerType)
        {
            _handNumber = handNumber;
            _slotNumber = slotNumber;
            _towerType = towerType;
        }
    }
}