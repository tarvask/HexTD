using System;
using System.Collections.Generic;
using Match.Field.Hexagonal;
using PathSystem;
using Tools;

namespace Match
{
    public class MatchShortParameters : BaseDisposable
    {
        private readonly FieldHex[] _hexes;
        private readonly PathData.SavePathData[] _paths;
        private readonly int _silverCoinsCount;
        
        private PlayerHandParams _handParams;
        
        private readonly byte[] _blockersNetwork;
        private readonly byte[] _cellsNetwork;
        private readonly Dictionary<byte, int> _spellsWithCountsNetwork;
        
        public FieldHex[] Hexes => _hexes;
        public PathData.SavePathData[] Paths => _paths;
        public int SilverCoinsCount => _silverCoinsCount;

        public PlayerHandParams HandParams => _handParams;
        public byte[] CellsNetwork => _cellsNetwork;
        public byte[] BlockersNetwork => _blockersNetwork;
        public Dictionary<byte, int> SpellsWithCountsNetwork => _spellsWithCountsNetwork;

        protected MatchShortParameters(FieldHex[] hexes,
            PathData.SavePathData[] paths,
            int silverCoinsCount,
            PlayerHandParams handParams)
        {
            // fill cells from linear array
            _hexes = new FieldHex[hexes.Length];
            Array.Copy(hexes, _hexes, hexes.Length);
            
            // fill paths from linear array
            _paths = new PathData.SavePathData[paths.Length];
            Array.Copy(paths, _paths, paths.Length);
            
            // hand
            _handParams = handParams;
            // currency and magic
            _silverCoinsCount = silverCoinsCount;

            // fill cells in photon-compatible form
            _cellsNetwork = new byte[hexes.Length];

            for (int hexIndex = 0; hexIndex < hexes.Length; hexIndex++)
            {
                _cellsNetwork[hexIndex] = (byte)_hexes[hexIndex].FieldHexType;
            }
        }

        public void ChangeHand(PlayerHandParams playerHand)
        {
            _handParams = playerHand;
        }
    }
}