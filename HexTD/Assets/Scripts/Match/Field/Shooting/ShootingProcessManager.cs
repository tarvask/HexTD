using System.Collections.Generic;
using BuffLogic;
using HexSystem;
using Match.Field.AttackEffect;
using Match.Field.Hexagons;
using Match.Field.Shooting.SplashDamage;
using Match.Field.Shooting.TargetFinding;
using Match.Field.Tower;
using Match.Field.VFX;
using Match.Wave;
using Tools;
using Tools.Interfaces;
using UniRx;
using UnityEngine;

namespace Match.Field.Shooting
{
    public class ShootingProcessManager : BaseDisposable, IOuterLogicUpdatable, IOuterViewUpdatable
    {
        public struct Context
        {
            public FieldModel FieldModel { get; }
            public HexMapReachableService HexMapReachableService { get; }
            public FieldFactory Factory { get; }
            public BuffManager BuffManager { get; }
            public VfxManager VfxManager { get; }
            public IReadOnlyReactiveProperty<int> CurrentEngineFrameReactiveProperty { get; }

            public Context(FieldModel fieldModel, 
                HexMapReachableService hexMapReachableService,
                FieldFactory factory,
                BuffManager buffManager,
                VfxManager vfxManager,
                IReadOnlyReactiveProperty<int> currentEngineFrameReactiveProperty)
            {
                FieldModel = fieldModel;
                HexMapReachableService = hexMapReachableService;
                Factory = factory;
                BuffManager = buffManager;
                VfxManager = vfxManager;
                CurrentEngineFrameReactiveProperty = currentEngineFrameReactiveProperty;
            }
        }

        private const byte MaxHitsPerFrame = 128;
        private readonly Context _context;
        private readonly TargetFinder _targetFinder;
        
        private readonly List<ProjectileController> _hittingProjectiles;
        private readonly List<ProjectileController> _endSplashingProjectiles;
        private readonly Dictionary<int, List<int>> _shootablesWithAttackingTowers;
        private readonly List<TargetWithSqrDistancePair> _targetsWithSqrDistances;

        public ShootingProcessManager(Context context)
        {
            _context = context;
            
            _targetFinder = AddDisposable(new TargetFinder(_context.HexMapReachableService));
            
            _hittingProjectiles = new List<ProjectileController>(MaxHitsPerFrame);
            _endSplashingProjectiles = new List<ProjectileController>(MaxHitsPerFrame);
            
            _shootablesWithAttackingTowers = new Dictionary<int, List<int>>(WaveMobSpawnerCoordinator.MaxMobsInWave);
            _targetsWithSqrDistances = new List<TargetWithSqrDistancePair>(WaveMobSpawnerCoordinator.MaxMobsInWave);
        }

        public void OuterLogicUpdate(float frameLength)
        {
            UpdateShooters(frameLength);
            UpdateMovingProjectiles(frameLength);
            UpdateHittingProjectiles();
            UpdateSplashingProjectiles();
        }

        public void OuterViewUpdate(float frameLength)
        {
            foreach (KeyValuePair<int, ProjectileController> projectilePair in _context.FieldModel.ProjectilesContainer.Projectiles)
            {
                projectilePair.Value.VisualMove(frameLength);
            }
        }

        private void UpdateShooters(float frameLength)
        {
            foreach (var shooter in _context.FieldModel.Shooters)
            {
                if (!shooter.IsAttackReady)
                    continue;
                
                if (Aim(shooter))
                    Shoot(shooter);
            }
        }

        private void UpdateMovingProjectiles(float frameLength)
        {
            foreach (KeyValuePair<int, ProjectileController> projectilePair in _context.FieldModel.ProjectilesContainer.Projectiles)
            {
                // if reached before movement
                if (projectilePair.Value.HasReachedTarget)
                    continue;

                Vector3 targetPosition;

                switch (projectilePair.Value.SplashShootType)
                {
                    case SplashShootType.ToTarget:
                        if (_context.FieldModel.Targets.TryGetTargetByIdAndType(projectilePair.Value.TargetId, 
                                projectilePair.Value.BaseAttackEffect.AttackTargetType,
                                out ITarget target))
                            targetPosition = target.Position;
                        else
                            targetPosition = projectilePair.Value.CurrentTargetPosition;
                        break;
                    
                    case SplashShootType.UnderSelf:
                        targetPosition = projectilePair.Value.CurrentPosition;
                        break;
                        
                    default:
                        Debug.LogError("Splash Shoot Type is Undefined!");
                        targetPosition = projectilePair.Value.CurrentTargetPosition;
                        break;
                }
                
                projectilePair.Value.LogicMove(targetPosition, frameLength);

                if (projectilePair.Value.HasReachedTarget)
                    _hittingProjectiles.Add(projectilePair.Value);
            }
        }

        private void UpdateHittingProjectiles()
        {
            foreach (ProjectileController projectile in _hittingProjectiles)
            {
                GetTargetsWithSqrDistancesForProjectile(projectile, _targetsWithSqrDistances);

                // handle hit for every target in damage area
                foreach (TargetWithSqrDistancePair targetWithSqrDistance in _targetsWithSqrDistances)
                {
                    HandleHitShootable(projectile, 
                        targetWithSqrDistance.Target,
                        targetWithSqrDistance.SqrDistance);
                }

                if (projectile.HasSplashDamage)
                {
                    int hexRadius = ((BaseSplashAttack)projectile.BaseAttackEffect).SplashRadiusInHex;
                    float radius = _context.FieldModel.HexPositionConversionService.GetRadiusFromRadiusInHex(hexRadius);
                    projectile.ShowSplash(radius);
                }
                else if (projectile.HasTargetVolumeDamage)
                {
                    float unitsRadius = ((BaseSingleAttack)projectile.BaseAttackEffect).SplashRadiusInUnits;
                    projectile.ShowSplash(unitsRadius);
                }
                else
                    projectile.ShowSingleHit();
                
                // clear because it's used for every projectile
                _targetsWithSqrDistances.Clear();
            }
            
            _hittingProjectiles.Clear();
        }

        private void UpdateSplashingProjectiles()
        {
            // check splash ending
            foreach (KeyValuePair<int, ProjectileController> projectilePair in _context.FieldModel.ProjectilesContainer.Projectiles)
            {
                if (!projectilePair.Value.HasPlayedSplash)
                    continue;
                
                _endSplashingProjectiles.Add(projectilePair.Value);
            }

            // dispose every fully splashed projectile
            foreach (ProjectileController projectile in _endSplashingProjectiles)
            {
                Debug.Log($"Removed projectile with id={projectile.Id} with target {projectile.TargetId} in position {projectile.CurrentPosition} on frame {_context.CurrentEngineFrameReactiveProperty.Value}");
                _context.FieldModel.ProjectilesContainer.Remove(projectile.Id);
                projectile.Dispose();
            }
            
            _endSplashingProjectiles.Clear();
        }

        private bool Aim(IShooter tower)
        {
            return tower.TryFindTarget(_targetFinder, _context.FieldModel.Targets);
        }

        private void Shoot(IShooter tower)
        {
            var projectileController = tower.CreateAndInitProjectile(_context.Factory);
            _context.FieldModel.ProjectilesContainer.Add(projectileController);
        }

        private void GetTargetsWithSqrDistancesForProjectile(ProjectileController projectile,
            List<TargetWithSqrDistancePair> targetWithSqrDistancePairs)
        {
            if (projectile.HasSplashDamage || projectile.HasTargetVolumeDamage)
            {
                SplashTargetDistanceComputer.GetTargetsWithSqrDistances(projectile, 
                    _context.FieldModel.HexPositionConversionService,
                    _context.FieldModel.Targets.GetTargetsByPosition(projectile.BaseAttackEffect.AttackTargetType),
                    targetWithSqrDistancePairs);
            }
            else if (_context.FieldModel.Targets.TryGetTargetByIdAndType(projectile.TargetId, 
                         projectile.BaseAttackEffect.AttackTargetType, out var target))
            {
                // distance is 0 for straight hit
                targetWithSqrDistancePairs.Add(new TargetWithSqrDistancePair(target, 0));
            }
        }

        private void HandleHitShootable(ProjectileController projectile, ITarget targeter, float sqrDistance)
        {
            projectile.BaseAttackEffect.ApplyAttackImpact(targeter, sqrDistance);
            projectile.BaseAttackEffect.ApplyAttackEffect(targeter, _context.BuffManager, _context.VfxManager);
            
            if (!_shootablesWithAttackingTowers.ContainsKey(targeter.TargetId))
                _shootablesWithAttackingTowers.Add(targeter.TargetId, new List<int>());
            
            _shootablesWithAttackingTowers[targeter.TargetId].Add(projectile.SpawnTowerId);
        }

        private int ComputeIfSplashDamage(int towerId, float damage,
            bool hasSplashDamage, float splashDamageRadius, bool hasProgressiveSplash, float sqrDistance)
        {
            if (hasSplashDamage)
            {
                float splashDistanceDecreaseCoeff = hasProgressiveSplash
                    ? 1 - sqrDistance / splashDamageRadius
                    : 1;
                return Mathf.CeilToInt(damage * splashDistanceDecreaseCoeff);
            }
            
            return Mathf.CeilToInt(damage);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _hittingProjectiles.Clear();
            _endSplashingProjectiles.Clear();
            _shootablesWithAttackingTowers.Clear();
            _targetsWithSqrDistances.Clear();
        }
    }
}