using HexSystem;
using Match.Field;
using Match.Field.Hexagonal;
using Match.Wave;
using PathSystem;

namespace Match
{
    public class MatchParameters : MatchShortParameters
    {
        private readonly WaveParams[] _waves;
        
        public WaveParams[] Waves => _waves;

        public MatchParameters(FieldHex[] hexModels, PathData.SavePathData[] paths, MatchConfig config, PlayerHandParams handParams)
            : base(hexModels, paths, config.SilverCoinsCount, handParams)
        {
            _waves = config.Waves;
        }

        public MatchParameters(WaveParams[] waves, 
            FieldHex[] hexTypes,
            int silverCoinsCount,
            PlayerHandParams handParams) : base(hexTypes, null, silverCoinsCount, handParams)
        {
            _waves = waves;
        }
    }
}