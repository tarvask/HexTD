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

        public void SetLevel(TowerLevelParams levelParams, float timeLabel)
        {
            _level = levelParams.LevelRegularParams.Data.Level;
            _constructionTimeLabel = timeLabel;
            
            CheckAndAddSplashAbility(levelParams.PassiveLevelAbilities.Abilities, false);
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
        
        public void SetBlockerTarget(int targetId)
        {
            _targetId = targetId;
            _isTargetBlocker = true;
        }
        
        public void UnsetBlockerTarget(int targetId)
        {
            if (_targetId == targetId)
            {
                _targetId = -1;
                _isTargetBlocker = false;
            }
        }

        public void ResetShootingTimer()
        {
            _shootingTimer = 0;
        }

        public void ResetTarget()
        {
            _targetId = -1;
        }

        public void CheckAndAddSplashAbility(AbstractAbilityMarker[] passiveAbilities, bool isArtifactEffect)
        {
            // AbstractAbilityMarker splashDamageAbility = Array.Find(passiveAbilities,
            //     ability => ability is SplashDamageAbilityMarker);
            //
            // // can be decided by isArtifact condition, maybe choose the strongest
            // if (splashDamageAbility != null)
            // {
            //     _hasSplashDamage = true;
            //
            //     SplashDamageBuffParameters splashDamageParameters = (SplashDamageBuffParameters)splashDamageAbility.AbilityToBuff();
            //     _splashDamageRadius = splashDamageParameters.BuffValue;
            //     _hasProgressiveSplashDamage = splashDamageParameters.HasProgressiveSplash;
            //
            //     if (!isArtifactEffect)
            //         _hasSplashInitially = true;
            // }
        }
        
        public void CheckAndRemoveSplashAbility(AbstractAbilityMarker[] passiveAbilities, bool isArtifactEffect)
        {
            // AbstractAbilityMarker splashDamageAbility = Array.Find(passiveAbilities,
            //     ability => ability is SplashDamageAbilityMarker);
            //
            // if (splashDamageAbility != null && _hasSplashDamage && isArtifactEffect)
            // {
            //     _hasSplashDamage = false;
            // }
        }
    }
}