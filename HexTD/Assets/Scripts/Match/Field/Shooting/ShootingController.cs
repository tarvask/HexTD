using System.Collections.Generic;
using BuffLogic;
using Match.Field.Hexagons;
using Match.Field.Shooting.SplashDamage;
using Match.Field.Shooting.TargetFinding;
using Match.Field.Tower;
using Match.Wave;
using Tools;
using Tools.Interfaces;
using UnityEngine;

namespace Match.Field.Shooting
{
    public class ShootingController : BaseDisposable, IOuterLogicUpdatable, IOuterViewUpdatable
    {
        public struct Context
        {
            public FieldModel FieldModel { get; }
            public HexMapReachableService HexMapReachableService { get; }
            public FieldFactory Factory { get; }
            public BuffManager BuffManager { get; }

            public Context(FieldModel fieldModel, 
                HexMapReachableService hexMapReachableService,
                FieldFactory factory,
                BuffManager buffManager)
            {
                FieldModel = fieldModel;
                HexMapReachableService = hexMapReachableService;
                Factory = factory;
                BuffManager = buffManager;
            }
        }

        private const byte MaxHitsPerFrame = 128;
        private readonly Context _context;
        private readonly TargetFinder _targetFinder;
        
        private readonly List<ProjectileController> _hittingProjectiles;
        private readonly List<ProjectileController> _endSplashingProjectiles;
        private readonly Dictionary<int, List<int>> _shootablesWithAttackingTowers;
        private List<TargetWithSqrDistancePair> _targetsWithSqrDistances;

        public ShootingController(Context context)
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
            foreach (KeyValuePair<int, ProjectileController> projectilePair in _context.FieldModel.Projectiles)
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
            foreach (KeyValuePair<int, ProjectileController> projectilePair in _context.FieldModel.Projectiles)
            {
                // if reached before movement
                if (projectilePair.Value.HasReachedTarget)
                    continue;

                Vector3 targetPosition;
                
                if (_context.FieldModel.Targets.TryGetTargetByIdAndType(projectilePair.Value.TargetId, 
                        projectilePair.Value.BaseAttackEffect.AttackTargetType,
                        out ITargetable target))
                    targetPosition = target.Position;
                else
                {
                    // fly to last target position
                    targetPosition = projectilePair.Value.CurrentTargetPosition;
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
                if (projectile.HasSplashDamage)
                {
                    SplashTargetDistanceComputer.GetTargetsWithSqrDistances(projectile, 
                        _context.FieldModel.Targets.IterateTargetsWithTypes(projectile.BaseAttackEffect.AttackTargetType),
                        ref _targetsWithSqrDistances);
                }
                else if (_context.FieldModel.Targets.TryGetTargetByIdAndType(projectile.TargetId, 
                             projectile.BaseAttackEffect.AttackTargetType, out var target))
                {
                    // distance is 0 for straight hit
                    _targetsWithSqrDistances.Add(new TargetWithSqrDistancePair(target, 0));
                }

                // handle hit for every target in damage area
                foreach (TargetWithSqrDistancePair targetWithSqrDistance in _targetsWithSqrDistances)
                {
                    HandleHitShootable(projectile, 
                        targetWithSqrDistance.Target,
                        targetWithSqrDistance.SqrDistance);
                }

                if (projectile.HasSplashDamage)
                    projectile.ShowSplash();
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
            foreach (KeyValuePair<int, ProjectileController> projectilePair in _context.FieldModel.Projectiles)
            {
                if (!projectilePair.Value.HasPlayedSplash)
                    continue;
                
                _endSplashingProjectiles.Add(projectilePair.Value);
            }

            // dispose every fully splashed projectile
            foreach (ProjectileController projectile in _endSplashingProjectiles)
            {
                _context.FieldModel.RemoveProjectile(projectile.Id);
                projectile.Dispose();
            }
            
            _endSplashingProjectiles.Clear();
        }

        private bool Aim(IShootable tower)
        {
            return tower.TryFindTarget(_targetFinder, _context.FieldModel.Targets);
        }

        private void Shoot(IShootable tower)
        {
            var projectileController = tower.CreateAndInitProjectile(_context.Factory);
            _context.FieldModel.AddProjectile(projectileController);
        }

        private void HandleHitShootable(ProjectileController projectile, ITargetable hitTargetable, float sqrDistance)
        {
            projectile.BaseAttackEffect.ApplyAttack(hitTargetable, _context.BuffManager);
            
            if (!_shootablesWithAttackingTowers.ContainsKey(hitTargetable.TargetId))
                _shootablesWithAttackingTowers.Add(hitTargetable.TargetId, new List<int>());
            
            _shootablesWithAttackingTowers[hitTargetable.TargetId].Add(projectile.SpawnTowerId);
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