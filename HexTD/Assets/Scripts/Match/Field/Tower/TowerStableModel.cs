using HexSystem;
using Tools;

namespace Match.Field.Tower
{
    public class TowerStableModel : BaseDisposable
    {
        // main
        private int _level;
        private float _constructionTimeLabel;
        private int _targetId;
        private TowerState _towerState;
        private Hex2d _hexPosition;
        
        public int Level => _level;
        public float ConstructionTimeLabel => _constructionTimeLabel;
        public int TargetId => _targetId;
        public bool CanShoot => _towerState == TowerState.Active;
        public bool IsAlive => _towerState != TowerState.Removing && _towerState != TowerState.ToDispose; 
        public bool IsConstructing => _towerState == TowerState.Constructing;
        public bool IsReadyToRelease => _towerState == TowerState.ToRelease;
        public bool IsReadyToDispose => _towerState == TowerState.ToDispose;
        public Hex2d HexPosition => _hexPosition;

        public TowerStableModel()
        {
            _level = 1;
        }

        public void SetLevel(int level, float timeLabel)
        {
            _level = level;
            _constructionTimeLabel = timeLabel;
        }

        public void SetState(TowerState towerState)
        {
            _towerState = towerState;
        }

        public void SetTarget(int targetId)
        {
            _targetId = targetId;
        }

        public void ResetTarget()
        {
            _targetId = -1;
        }

        public void SetHexPosition(Hex2d hexPosition)
        {
            _hexPosition = hexPosition;
        }
    }
}