using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HexSystem;
using Match.Field.Buff;
using Match.Field.Mob;
using Match.Field.Shooting;
using Match.Field.Shooting.TargetFinding;
using Match.Field.State;
using Match.Field.Tower.TowerConfigs;
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
            public TowerConfigNew TowerConfig { get; }
            public TowerView View { get; }
            public Sprite Icon { get; }
            public int TowerRemovingDuration { get; }

            public Context(int id, Hex2d position, TowerConfigNew towerConfig, 
                TowerView view, Sprite icon, int towerRemovingDuration)
            {
                Id = id;
                TowerConfig = towerConfig;
                View = view;
                Icon = icon;
                Position = position;
                TowerRemovingDuration = towerRemovingDuration;
            }
        }

        private readonly Context _context;
        private readonly TowerShootModel _shootModel;
        private readonly TowerStableModel _stableModel;
        private readonly TowerReactiveModel _reactiveModel;
        // effects that are applied to mobs after shot (firing, icing, slowing)
        
        private TowerLevelConfig CurrentLevel => _context.TowerConfig.TowerLevelConfigs[_stableModel.Level - 1];
        private TowerLevelConfig NextLevel => _context.TowerConfig.TowerLevelConfigs[_stableModel.Level];
        private Vector3 Position => _context.View.transform.localPosition;

        public int Id => _context.Id;
        public Hex2d HexPosition => _context.Position;
        public bool IsReadyToShoot => _shootModel.IsReadyAttack;
        public bool HasTarget => _stableModel.TargetId > 0;
        public int TargetId => _stableModel.TargetId;
        public bool CanShoot => _stableModel.CanShoot;
        public bool IsAlive => _stableModel.IsAlive;
        public bool IsReadyToRelease => _stableModel.IsReadyToRelease;
        public bool IsReadyToDispose => _stableModel.IsReadyToDispose;
        public TowerType TowerType => _context.TowerConfig.RegularParameters.TowerType;
        public IReadOnlyReactiveProperty<int> KillsCount => _reactiveModel.KillsCountReactiveProperty;
        public ProjectileView ProjectilePrefab => _context.TowerConfig.TowerAttackConfig.TowerAttacks[0].ProjectileView;
        public Sprite Icon => _context.Icon;

        public TowerController(Context context)
        {
            _context = context;

            _shootModel = AddDisposable(new TowerShootModel(_context.TowerConfig.TowerAttackConfig));
            _stableModel = AddDisposable(new TowerStableModel());
            _reactiveModel = AddDisposable(new TowerReactiveModel());
            
            //_buffsManager = AddDisposable(new TowerBuffsManager(new TowerBuffsManager.Context(
            //    _reactiveModel.AttackPowerReactiveProperty,
            //    _reactiveModel.AttackRadiusReactiveProperty,
            //    _reactiveModel.ReloadTimeReactiveProperty)));
            _context.View.SetType(_context.TowerConfig.RegularParameters.TowerName);
        }

        public void OuterLogicUpdate(float frameLength)
        {
            _shootModel.OuterLogicUpdate(frameLength);
            //_buffsManager.OuterLogicUpdate(frameLength);
        }

        // TODO: fix abilities' behaviour
        public void SetLevel(int newLevel, int constructionDuration = -1)
        {
            // no construction for duration = 0
            if (constructionDuration < 0)
                SetConstructing(CurrentLevel.BuildTime);
            else if (constructionDuration > 0)
                SetConstructing(constructionDuration * 0.001f);
            
            _stableModel.SetLevel(newLevel, Time.time);
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
            if (_stableModel.Level < _context.TowerConfig.TowerLevelConfigs.Count)
            {
                SetLevel(_stableModel.Level+1);
            }
        }

        public void Release()
        {
            _context.View.SetLevel(_stableModel.Level);
            _stableModel.SetState(TowerState.Active);
        }

        public TowerShortParams GetShortParams()
        {
            return new TowerShortParams(_context.TowerConfig.RegularParameters.TowerType, _stableModel.Level);
        }

        public bool FindTarget(TargetFinder targetFinder, IReadOnlyDictionary<int, List<MobController>> mobs)
        {
            if (!_shootModel.TryReleaseTowerAttack(false, out var towerAttack))
                return false;
            
            int targetId = targetFinder.GetTargetWithTacticInRange(mobs,
                _context.TowerConfig.RegularParameters.ReachableAttackTargetFinderType,
                _context.TowerConfig.RegularParameters.TargetFindingTacticType, 
                HexPosition, towerAttack.AttackRadiusInHex,
                _context.TowerConfig.RegularParameters.PreferUnbuffedTargets);//, _activeAbilities);
            _stableModel.SetTarget(targetId);
            return HasTarget;
        }

        public ProjectileController Shoot(FieldFactory factory)
        {
            if (!_shootModel.TryReleaseTowerAttack(true, out var towerAttack))
                throw new Exception("Try to make attack but no ones ready yet!");
            
            ProjectileController projectile = factory.CreateProjectile(
                towerAttack,
                Position,
                _stableModel.HasSplashDamage, _stableModel.SplashDamageRadius, _stableModel.HasProgressiveSplashDamage,
                _context.Id, _stableModel.TargetId);

            if (_context.TowerConfig.RegularParameters.ResetTargetEveryShot && !_stableModel.IsTargetBlocker)
                _stableModel.ResetTarget();

            return projectile;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            Object.Destroy(_context.View.gameObject);
        }

        public void IncreaseScore()
        {
            _reactiveModel.IncreaseKills();
        }
        
        public static int GetTowerSellPrice(TowerLevelConfigsDictionary towerLevels, int towerLevel)
        {
            // int sellPrice = 0;
            //
            // for (int levelIndex = 0; levelIndex < towerLevel; levelIndex++)
            // {
            //     sellPrice += towerParameters.Levels[levelIndex].LevelRegularParams.Data.Price;
            // }
            //
            // return Mathf.CeilToInt(sellPrice * 0.5f);
            return towerLevels[towerLevel - 1].RefundPrice;
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

        public void ShowSelection()
        {
            
        }

        public void HideSelection()
        {
            
        }

        public void LoadState(in PlayerState.TowerState towerState)
        {
            SetLevel(towerState.Level, towerState.ConstructionTime);
        }

        public PlayerState.TowerState GetTowerState()
        {
            return new PlayerState.TowerState(_context.Id, 
                (byte)_context.Position.Q, (byte)_context.Position.R,
                _context.TowerConfig.RegularParameters.TowerType,
                (byte)_stableModel.Level,
                // save remaining time
                _stableModel.IsConstructing ? (int)((CurrentLevel.BuildTime - (Time.time - _stableModel.ConstructionTimeLabel)) * 1000) : 0);
        }
    }
}