using System.Collections.Generic;
using Match.Field;
using Match.Field.Services;
using Tools;
using UnityEngine;

namespace Match
{
    public class MatchShortParameters : BaseDisposable
    {
        public const int FieldWidth = 9;
        public const int FieldHeight = 6;
        private readonly FieldCellType[,] _cells;
        private readonly Vector3[] _wayPoints;
        private readonly int _silverCoinsCount;
        
        private PlayerHandParams _handParams;
        
        private readonly byte[] _blockersNetwork;
        private readonly byte[] _cellsNetwork;
        private readonly Dictionary<byte, int> _spellsWithCountsNetwork;
        
        public FieldCellType[,] Cells => _cells;
        public Vector3[] WayPoints => _wayPoints;
        public int SilverCoinsCount => _silverCoinsCount;

        public PlayerHandParams HandParams => _handParams;
        public byte[] CellsNetwork => _cellsNetwork;
        public byte[] BlockersNetwork => _blockersNetwork;
        public Dictionary<byte, int> SpellsWithCountsNetwork => _spellsWithCountsNetwork;

        protected MatchShortParameters(FieldCellType[] cells,
            int silverCoinsCount,
            PlayerHandParams handParams)
        {
            // fill cells from linear array
            _cells = new FieldCellType[FieldHeight, FieldWidth];

            for (int cellIndex = 0; cellIndex < FieldHeight * FieldWidth; cellIndex++)
            {
                _cells[cellIndex / FieldWidth, cellIndex % FieldWidth] = cells[cellIndex];
            }
            
            // compute waypoints
            _wayPoints = RoadWayPointsFinder.GetWaypointsFromField(_cells);
            // hand
            _handParams = handParams;
            // currency and magic
            _silverCoinsCount = silverCoinsCount;

            // fill cells in photon-compatible form
            _cellsNetwork = new byte[FieldWidth * FieldHeight];

            for (int yCell = 0; yCell < FieldHeight; yCell++)
            {
                for (int xCell = 0; xCell < FieldWidth; xCell++)
                {
                    _cellsNetwork[yCell * FieldWidth + xCell] = (byte)_cells[yCell, xCell];
                }
            }
        }

        public void ChangeHand(PlayerHandParams playerHand)
        {
            _handParams = playerHand;
        }
    }
}