using Match.Field.Tower.TowerConfigs;
using Tools;

namespace Match.Field.Tower
{
    public class TowerStableModel : BaseDisposable
    {
        // main
        private int _level;
        private float _shootingTimer;
        private float _constructionTimeLabel;
        private int _targetId;
        private bool _isTargetBlocker;
        private TowerState _towerState;
        
        // splash
        private bool _hasSplashDamage;
        private float _splashDamageRadius;
        private bool _hasProgressiveSplashDamage;
        private bool _hasSplashInitially;

        public int Level => _level;
        public float ShootingTimer => _shootingTimer;
        public float ConstructionTimeLabel => _constructionTimeLabel;
        public int TargetId => _targetId;
        public bool IsTargetBlocker => _isTargetBlocker;
        public bool CanShoot => _towerState == TowerState.Active;
        public bool IsAlive => _towerState != TowerState.Removing && _towerState != TowerState.ToDispose; 
        public bool IsConstructing => _towerState == TowerState.Constructing;
        public bool IsReadyToRelease => _towerState == TowerState.ToRelease;
        public bool IsReadyToDispose => _towerState == TowerState.ToDispose;

        public bool HasSplashDamage => _hasSplashDamage;
        public float SplashDamageRadius => _splashDamageRadius;
        public bool HasProgressiveSplashDamage => _hasProgressiveSplashDamage;

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

        public void UpdateShootingTimer(float frameLength)
        {
            _shootingTimer += frameLength;
        }

        public void SetTarget(int targetId)
        {
            _targetId = targetId;
            _isTargetBlocker = false;
        }

        public void ResetShootingTimer()
        {
            _shootingTimer = 0;
        }

        public void ResetTarget()
        {
            _targetId = -1;
        }
    }
}