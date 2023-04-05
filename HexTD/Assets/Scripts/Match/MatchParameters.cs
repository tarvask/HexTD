using Match.Field;
using Match.Wave;

namespace Match
{
    public class MatchParameters : MatchShortParameters
    {
        private readonly WaveParams[] _waves;
        
        public WaveParams[] Waves => _waves;

        public MatchParameters(MatchConfig config, PlayerHandParams handParams)
            : base(config.Cells,
                config.SilverCoinsCount, handParams)
        {
            _waves = config.Waves;
        }

        public MatchParameters(WaveParams[] waves, FieldCellType[] cells,
            int silverCoinsCount,
            PlayerHandParams handParams) : base(cells, silverCoinsCount, handParams)
        {
            _waves = waves;
        }
    }
}