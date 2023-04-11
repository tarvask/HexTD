using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HexSystem;
using Match.Field.Buff;
using Match.Field.Mob;
using Match.Field.Shooting;
using Match.Field.Shooting.TargetFinding;
using Match.Field.State;
using Tools;
using Tools.Interfaces;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Match.Field.Tower
{
    public class TowerController : BaseDisposable, IOuterLogicUpdatable
    {
        public struct Context
        {
            public int Id { get; }
            public Hex2d Position { get; }
            public TowerParameters Parameters { get; }
            public TowerView View { get; }
            public Sprite Icon { get; }
            public int TowerRemovingDuration { get; }

            public Context(int id, Hex2d position, TowerParameters parameters, 
                TowerView view, Sprite icon, int towerRemovingDuration)
            {
                Id = id;
                Parameters = parameters;
                View = view;
                Icon = icon;
                Position = position;
                TowerRemovingDuration = towerRemovingDuration;
            }
        }

        private const byte AverageAbilitiesCount = 6;
        private const float BlockerColliderRadius = 0.5f;
        private const byte ArtifactsMaxCount = 1;
        
        private readonly Context _context;
        private readonly TowerStableModel _stableModel;
        private readonly TowerReactiveModel _reactiveModel;
        // effects that are applied to mobs after shot (firing, icing, slowing)
        private readonly List<AbstractBuffParameters> _activeAbilities;
        private readonly List<AbstractBuffParameters> _towersInAreaAbilities;
        //private readonly TowerBuffsManager _buffsManager;
        
        private TowerLevelParams CurrentLevel => _context.Parameters.Levels[_stableModel.Level - 1];
        private TowerLevelParams NextLevel => _context.Parameters.Levels[_stableModel.Level];
        private float AttackRadiusAndBlockerColliderSqr => (CurrentRadius + BlockerColliderRadius) * (CurrentRadius + BlockerColliderRadius); 
        private Vector3 Position => _context.View.transform.localPosition;

        public int Id => _context.Id;
        public Hex2d HexPosition => _context.Position;
        public bool IsReadyToShoot => _stableModel.ShootingTimer >= CurrentReloadTime;
        public bool HasTarget => _stableModel.TargetId > 0;
        public int TargetId => _stableModel.TargetId;
        public bool CanShoot => _stableModel.CanShoot;
        public bool IsAlive => _stableModel.IsAlive;
        public bool IsReadyToRelease => _stableModel.IsReadyToRelease;
        public bool IsReadyToDispose => _stableModel.IsReadyToDispose;
        public TowerType TowerType => _context.Parameters.RegularParameters.Data.TowerType;
        public RaceType RaceType => _context.Parameters.RegularParameters.Data.RaceType;
        public int CurrentDamage => Mathf.CeilToInt(CurrentLevel.LevelRegularParams.Data.AttackPower); // Mathf.CeilToInt(_buffsManager.ParameterResultValue(BuffedParameterType.AttackPower));
        private int CurrentRadius => CurrentLevel.LevelRegularParams.Data.AttackRadiusInHexCount; //_buffsManager.ParameterResultValue(BuffedParameterType.AttackRadius);
        private float CurrentReloadTime => CurrentLevel.LevelRegularParams.Data.ReloadTime; //_buffsManager.ParameterResultValue(BuffedParameterType.ReloadTime);
        public IReadOnlyReactiveProperty<int> KillsCount => _reactiveModel.KillsCountReactiveProperty;
        public List<AbstractBuffParameters> MobsBuffs => _activeAbilities;
        public List<AbstractBuffParameters> BuffsForNeighbouringTowers => _towersInAreaAbilities;
        //public Dictionary<int, AbstractBuffModel> Buffs => _buffsManager.Buffs;
        public ProjectileView ProjectilePrefab => CurrentLevel.ProjectilePrefab;
        public Sprite Icon => _context.Icon;

        public TowerController(Context context)
        {
            _context = context;

            _stableModel = AddDisposable(new TowerStableModel());
            _reactiveModel = AddDisposable(new TowerReactiveModel());
            _activeAbilities = new List<AbstractBuffParameters>(AverageAbilitiesCount);
            _towersInAreaAbilities = new List<AbstractBuffParameters>(AverageAbilitiesCount);
            //_buffsManager = AddDisposable(new TowerBuffsManager(new TowerBuffsManager.Context(
            //    _reactiveModel.AttackPowerReactiveProperty,
            //    _reactiveModel.AttackRadiusReactiveProperty,
            //    _reactiveModel.ReloadTimeReactiveProperty)));
            _context.View.SetType(_context.Parameters.RegularParameters.Data.TowerName);
        }

        public void OuterLogicUpdate(float frameLength)
        {
            //_buffsManager.OuterLogicUpdate(frameLength);
        }

        // TODO: fix abilities' behaviour
        public void SetLevel(int newLevel, int constructionDuration = -1)
        {
            // no construction for duration = 0
            if (constructionDuration < 0)
                SetConstructing(_context.Parameters.Levels[newLevel - 1].LevelRegularParams.Data.BuildingTime);
            else if (constructionDuration > 0)
                SetConstructing(constructionDuration * 0.001f);
            
            _stableModel.SetLevel(_context.Parameters.Levels[newLevel - 1], Time.time);
            _reactiveModel.SetLevel(CurrentLevel);

            // update active abilities
            _activeAbilities.Clear();
            
            foreach (IAbility ability in CurrentLevel.ActiveLevelAbilities.Abilities)
            {
                if (!ability.IsConditional || CheckAbilityConditions(ability))
                    _activeAbilities.Add(ability.AbilityToBuff());
            }
            
            // update towers in area abilities
            _towersInAreaAbilities.Clear();

            foreach (IAbility ability in CurrentLevel.TowersInAreaAbilities.Abilities)
            {
                if (ability.IsTowersInAreaApplied)
                    _towersInAreaAbilities.Add(ability.AbilityToBuff());
            }
            
            // update passive abilities
            //_buffsManager.ClearBuffs();
            
            foreach (IAbility ability in CurrentLevel.PassiveLevelAbilities.Abilities)
            {
                if (!ability.IsConditional || CheckAbilityConditions(ability))
                {
                    AbstractBuffParameters abilityParameters = ability.AbilityToBuff();
                    
                    //if (abilityParameters.BuffedParameterType != BuffedParameterType.Undefined)
                        //_buffsManager.AddBuff(ability.AbilityToBuff());
                }
            }
        }

        private void SetConstructing(float constructionTime)
        {
            _stableModel.SetState(TowerState.Constructing);
            _context.View.SetConstructing();
            
            Task.Run(async () =>
            {
                // milliseconds
                await Task.Delay((int)(constructionTime * 1000));
                _stableModel.SetState(TowerState.ToRelease);
            });
        }
        
        public void Upgrade()
        {
            if (_stableModel.Level < _context.Parameters.Levels.Length)
            {
                SetLevel(NextLevel.LevelRegularParams.Data.Level);
            }
        }

        public void Release()
        {
            _context.View.SetLevel(_stableModel.Level);
            _stableModel.SetState(TowerState.Active);
        }

        public TowerShortParams GetShortParams()
        {
            return new TowerShortParams(_context.Parameters.RegularParameters.Data.TowerType, _stableModel.Level);
        }

        public void UpdateTimer(float frameLength)
        {
            _stableModel.UpdateShootingTimer(frameLength);
        }

        public bool FindTarget(TargetFinder targetFinder, IReadOnlyDictionary<int, List<MobController>> mobs)
        {
            int targetId = targetFinder.GetTargetWithTacticInRange(mobs,
                _context.Parameters.RegularParameters.Data.ReachableAttackTargetFinderType,
                _context.Parameters.RegularParameters.Data.TargetFindingTacticType, 
                HexPosition, CurrentRadius,
                _context.Parameters.RegularParameters.Data.PreferUnbuffedTargets);//, _activeAbilities);
            _stableModel.SetTarget(targetId);
            return HasTarget;
        }

        public ProjectileController Shoot(FieldFactory factory)
        {
            ProjectileController projectile = factory.CreateProjectile(CurrentLevel.ProjectilePrefab, Position,
                CurrentLevel.LevelRegularParams.Data.ProjectileSpeed,
                _stableModel.HasSplashDamage, _stableModel.SplashDamageRadius, _stableModel.HasProgressiveSplashDamage,
                _context.Id, _stableModel.TargetId);
            _stableModel.ResetShootingTimer();

            if (_context.Parameters.RegularParameters.Data.ResetTargetEveryShot && !_stableModel.IsTargetBlocker)
                _stableModel.ResetTarget();

            return projectile;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            Object.Destroy(_context.View.gameObject);
            
            _activeAbilities.Clear();
            _towersInAreaAbilities.Clear();
        }

        public void IncreaseScore()
        {
            _reactiveModel.IncreaseKills();
        }
        
        public static int GetTowerSellPrice(TowerParameters towerParameters, int towerLevel)
        {
            int sellPrice = 0;
            
            for (int levelIndex = 0; levelIndex < towerLevel; levelIndex++)
            {
                sellPrice += towerParameters.Levels[levelIndex].LevelRegularParams.Data.Price;
            }

            return Mathf.CeilToInt(sellPrice * 0.5f);
        }

        public void AddBuff(AbstractBuffParameters buffParameters)
        {
            //_buffsManager.AddBuff(buffParameters);
        }
        
        public void RemoveBuff(BuffedParameterType buffedParameterType, byte buffSubtype)
        {
            //_buffsManager.RemoveBuff(buffedParameterType, buffSubtype);
        }

        // TODO: will be greatly extended, so a better approach is needed
        private bool CheckAbilityConditions(IAbility ability)
        {
            Type abilityType = ability.GetType();

            // if (abilityType == typeof(RoadBuffMarker))
            //     return !_context.IsCellNearRoad;
            
            throw new ArgumentException("Unknown conditional ability: " + abilityType);
        }

        public void SetRemoving()
        {
            _context.View.SetRemoving();
            _stableModel.SetState(TowerState.Removing);
            
            Task.Run(async () =>
            {
                // milliseconds
                await Task.Delay(_context.TowerRemovingDuration * 1000);
                _stableModel.SetState(TowerState.ToDispose);
            });
        }

        public void LoadState(in PlayerState.TowerState towerState)
        {
            SetLevel(towerState.Level, towerState.ConstructionTime);
        }

        public PlayerState.TowerState GetTowerState()
        {
            return new PlayerState.TowerState(_context.Id, 
                (byte)_context.Position.Q, (byte)_context.Position.R,
                _context.Parameters.RegularParameters.Data.TowerType,
                (byte)_stableModel.Level,
                // save remaining time
                _stableModel.IsConstructing ? (int)((CurrentLevel.LevelRegularParams.Data.BuildingTime - (Time.time - _stableModel.ConstructionTimeLabel)) * 1000) : 0);
        }
    }
}