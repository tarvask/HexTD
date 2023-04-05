using Match.Field.Tower;

namespace Match
{
    public class PlayerHandParams
    {
        private readonly TowerType[] _towers;
        //[SerializeField] private object _stuff;

        private readonly byte[] _towersNetwork;

        public TowerType[] Towers => _towers;
        public byte[] TowersNetwork => _towersNetwork;

        public PlayerHandParams(TowerType[] towers)
        {
            _towers = towers;

            _towersNetwork = new byte[_towers.Length];

            for (int i = 0; i < _towers.Length; i++)
            {
                _towersNetwork[i] = (byte) _towers[i];
            }
        }
    }
}