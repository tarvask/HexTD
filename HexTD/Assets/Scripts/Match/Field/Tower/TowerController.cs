using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using HexSystem;
using Match.Field.AttackEffect;
using Match.Field.Hexagons;
using Match.Field.Shooting;
using Match.Field.Shooting.TargetFinding;
using Match.Field.State;
using Match.Field.Tower.TowerConfigs;
using Match.Serialization;
using Services;
using Tools.Interfaces;
using UI.ScreenSpaceOverlaySystem;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Match.Field.Tower
{
    public class TowerController : BaseTargetEntity, IOuterLogicUpdatable, IShooter, ISerializableToNetwork
    {
        public struct Context
        {
            public int Id { get; }
            public int TargetId { get; }
            public Hex2d HexPosition { get; }
            public TowerConfigNew TowerConfig { get; }
            public TowerView View { get; }
            public Sprite Icon { get; }
            public int TowerRemovingDuration { get; }
            public HexMapReachableService HexMapReachableService { get; }
            
            public ReactiveCommand<IReadOnlyCollection<Hex2d>> EnableHexesHighlightReactiveCommand { get; }
            public ReactiveCommand RemoveAllHexesHighlightsReactiveCommand { get; }

            public Context(int id, int targetId, Hex2d hexPosition, TowerConfigNew towerConfig, 
                TowerView view, Sprite icon, int towerRemovingDuration,
                HexMapReachableService hexMapReachableService,
                ReactiveCommand<IReadOnlyCollection<Hex2d>> enableHexesHighlightReactiveCommand ,
                ReactiveCommand removeAllHexesHighlightsReactiveCommand
                )
            {
                Id = id;
                TargetId = targetId;
                TowerConfig = towerConfig;
                View = view;
                Icon = icon;
                HexPosition = hexPosition;
                TowerRemovingDuration = towerRemovingDuration;
                HexMapReachableService = hexMapReachableService;
                EnableHexesHighlightReactiveCommand = enableHexesHighlightReactiveCommand;
                RemoveAllHexesHighlightsReactiveCommand = removeAllHexesHighlightsReactiveCommand;
            }
        }

        private readonly Context _context;
        private readonly EntityShootModel _shootModel;
        private readonly TowerStableModel _stableModel;
        private readonly TowerReactiveModel _reactiveModel;
        
        private TowerLevelConfig CurrentLevel => _context.TowerConfig.TowerLevelConfigs[_stableModel.Level];
        public override BaseReactiveModel BaseReactiveModel => _reactiveModel;

        public int Id => _context.Id;
        public override Hex2d HexPosition => _stableModel.HexPosition;
        public override Vector3 Position => _context.View.transform.localPosition;

        public bool IsAttackReady => _shootModel.IsReadyAttack && CanShoot;
        public bool HasTarget => _stableModel.CurrentTargetId > 0;
        public override int TargetId => _context.TargetId;
        public bool CanShoot => _stableModel.CanShoot;
        public bool IsAlive => _stableModel.IsAlive;
        public bool IsReadyToRelease => _stableModel.IsReadyToRelease;
        public bool IsReadyToDispose => _stableModel.IsReadyToDispose;
        public TowerType TowerType => _context.TowerConfig.RegularParameters.TowerType;
        public byte MaxEnemyBlocked => _context.TowerConfig.RegularParameters.MaxEnemyBlocked;
        public Sprite Icon => _context.Icon;
        public override ITargetView TargetView => _context.View;

        public TowerController(Context context)
        {
            _context = context;

            _shootModel = AddDisposable(new EntityShootModel(_context.TowerConfig.AttacksConfig));
            _stableModel = AddDisposable(new TowerStableModel());
            _reactiveModel = AddDisposable(new TowerReactiveModel(CurrentLevel.HealthPoint, _context.TargetId));
            
            _context.View.SetType(_context.TowerConfig.RegularParameters.TowerName);
            _stableModel.SetHexPosition(_context.HexPosition);
        }

        public void OuterLogicUpdate(float frameLength)
        {
            _shootModel.OuterLogicUpdate(frameLength);
        }

        // TODO: fix abilities' behaviour
        public void SetLevel(int newLevel, int constructionDuration = -1)
        {
            // no construction for duration = 0
            if (constructionDuration < 0)
                SetConstructing(CurrentLevel.BuildTime);
            else
                SetConstructing(constructionDuration * 0.001f);
            
            _stableModel.SetLevel(newLevel, Time.time);
        }

        private void SetConstructing(float constructionTime)
        {
            _stableModel.SetState(TowerState.Constructing);
            _context.View.SetConstructing();
            
            Task.Run(async () =>
            {
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

        public override void Heal(float heal)
        {
            _reactiveModel.SetHealth(_reactiveModel.Health.Value.CurrentValue + heal);
        }

        public override void Hurt(float damage)
        {
            _reactiveModel.SetHealth(_reactiveModel.Health.Value.CurrentValue - damage);
        }

        public TowerShortParams GetShortParams()
        {
            return new TowerShortParams(_context.TowerConfig.RegularParameters.TowerType, _stableModel.Level);
        }

        public bool TryFindTarget(TargetFinder targetFinder, TargetContainer targetContainer)
        {
            if (!_shootModel.TryGetTowerAttack(out var towerAttack))
                return false;

            if (_shootModel.IsSplashAttackReady)
                return true;
            
            int targetId = targetFinder.GetTargetWithTacticInRange(
                targetContainer.GetTargetsByPosition(towerAttack.AttackTargetType),
                    towerAttack.AttackRangeType,
                    _context.TowerConfig.RegularParameters.TargetFindingTacticType, 
                    HexPosition, ((BaseSingleAttack)towerAttack).AttackRadiusInHex,
                    _context.TowerConfig.RegularParameters.PreferUnbuffedTargets);
                            
            _stableModel.SetTarget(targetId);
            
            return HasTarget;
        }

        public ProjectileController CreateAndInitProjectile(FieldFactory factory)
        {
            if (!_shootModel.TryGetTowerAttack(out var towerAttack))
                throw new Exception("Try to make attack but no ones ready yet!");

            ProjectileController projectile = factory.CreateProjectile(
                towerAttack,
                _shootModel.ReadyTowerIndex,
                Position,
                _shootModel.IsSplashAttackReady,
                _context.Id, _stableModel.CurrentTargetId);
            
            _shootModel.ReloadCurrentAttack();

            if (_context.TowerConfig.RegularParameters.ResetTargetEveryShot)
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
            return towerLevels[towerLevel].RefundPrice;
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
            int attackRadius;
            AttackRangeType attackRangeType;

            if (_context.TowerConfig.AttacksConfig.Attacks.Count > 0)
            {
                BaseSingleAttack attackConfig = _context.TowerConfig.AttacksConfig.Attacks[0];
                attackRadius = attackConfig.AttackRadiusInHex;
                attackRangeType = attackConfig.AttackRangeType;
            }
            else if (_context.TowerConfig.AttacksConfig.SplashAttacks.Count > 0)
            {
                BaseSplashAttack splashAttackConfig = _context.TowerConfig.AttacksConfig.SplashAttacks[0];
                attackRadius = splashAttackConfig.SplashRadiusInHex;
                attackRangeType = splashAttackConfig.AttackRangeType;
            }
            else
            {
                throw new ArgumentException("Badly configured attacks for tower: " + _context.TowerConfig.RegularParameters.TowerName);
            }
            
            var hexes = _context.HexMapReachableService.GetInRangeMapByTargetFinderType(
                HexPosition,
                attackRadius,
                attackRangeType);

            _context.EnableHexesHighlightReactiveCommand.Execute(hexes);
        }

        public void HideSelection()
        {
            _context.RemoveAllHexesHighlightsReactiveCommand.Execute();
        }

        public void SetPlacing()
        {
            _stableModel.SetState(TowerState.Placing);
            _context.View.SetPlacing();
        }

        public void ChangePosition(Hex2d hexPosition, Vector3 worldPosition)
        {
            _stableModel.SetHexPosition(hexPosition);
            _context.View.transform.position = worldPosition;
        }

        public void LoadState(in PlayerState.TowerState towerState)
        {
            SetLevel(towerState.Level, towerState.ConstructionTime);
            _reactiveModel.SetHealth(towerState.CurrentHealth);
            _stableModel.SetTarget(towerState.CurrentTargetId);
        }

        public PlayerState.TowerState GetTowerState()
        {
            return new PlayerState.TowerState(_context.Id, 
                _context.TargetId,
                (byte)_stableModel.HexPosition.Q, (byte)_stableModel.HexPosition.R,
                _context.TowerConfig.RegularParameters.TowerType,
                (byte)_stableModel.Level,
                // save remaining time
                _stableModel.IsConstructing ? (int)((CurrentLevel.BuildTime - (Time.time - _stableModel.ConstructionTimeLabel)) * 1000) : 0,
                _reactiveModel.Health.Value.CurrentValue, _stableModel.CurrentTargetId);
        }
        
        public Hashtable ToNetwork()
        {
            return PlayerState.TowerState.TowerToHashtable(GetTowerState());
        }
        
        public static object FromNetwork(Hashtable hashtable, ConfigsRetriever configsRetriever, FieldFactory factory)
        {
            var towerState = PlayerState.TowerState.TowerFromHashtable(hashtable);    
            
            TowerConfigNew towerConfig = configsRetriever.GetTowerByType(towerState.Type);
            Hex2d towerHexPosition = new Hex2d(towerState.PositionQ, towerState.PositionR);
            TowerController towerController = factory.CreateTowerWithId(towerConfig,
                towerHexPosition, towerState.Id, towerState.TargetId);
            
            towerController.LoadState(towerState);

            return towerController;
        }
    }
}