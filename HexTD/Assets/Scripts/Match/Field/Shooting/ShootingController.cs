using System.Collections.Generic;
using BuffLogic;
using HexSystem;
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
        private readonly Dictionary<int, IShootable> _hitShootables;
        private readonly Dictionary<int, IShootable> _deadBodies;
        private readonly List<IShootable> _carrionBodies;
        private readonly Dictionary<int, List<int>> _shootablesWithAttackingTowers;
        private List<TargetWithSqrDistancePair> _targetsWithSqrDistances;
        private int _blockerTargetId;

        public ShootingController(Context context)
        {
            _context = context;
            
            _targetFinder = AddDisposable(new TargetFinder(_context.HexMapReachableService));
            
            _hittingProjectiles = new List<ProjectileController>(MaxHitsPerFrame);
            _endSplashingProjectiles = new List<ProjectileController>(MaxHitsPerFrame);
            _hitShootables = new Dictionary<int, IShootable>(WaveMobSpawnerCoordinator.MaxMobsInWave);
            
            _deadBodies = new Dictionary<int, IShootable>(WaveMobSpawnerCoordinator.MaxMobsInWave);
            _carrionBodies = new List<IShootable>(MaxHitsPerFrame);
            
            _shootablesWithAttackingTowers = new Dictionary<int, List<int>>(WaveMobSpawnerCoordinator.MaxMobsInWave);
            _targetsWithSqrDistances = new List<TargetWithSqrDistancePair>(WaveMobSpawnerCoordinator.MaxMobsInWave);
        }

        public void OuterLogicUpdate(float frameLength)
        {
            UpdateShooters(frameLength);
            UpdateMovingProjectiles(frameLength);
            UpdateHittingProjectiles();
            UpdateSplashingProjectiles();
            UpdateHitShootables();
            UpdateDeadBodies();
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
            foreach (KeyValuePair<int, TowerController> towerPair in _context.FieldModel.Towers)
            {
                if (!towerPair.Value.CanShoot)
                    continue;
                
                towerPair.Value.UpdateTimer(frameLength);

                if (!towerPair.Value.IsReadyToShoot)
                    continue;
                
                if (Aim(towerPair.Value))
                    Shoot(towerPair.Value);
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
                
                if (_context.FieldModel.Shootables.TryGetValue(projectilePair.Value.TargetId, out IShootable target))
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
                    SplashTargetDistanceComputer.GetTargetsWithSqrDistances(projectile, _context.FieldModel.Shootables,
                        ref _targetsWithSqrDistances);
                }
                else if (_context.FieldModel.Shootables.ContainsKey(projectile.TargetId))
                {
                    // distance is 0 for straight hit
                    _targetsWithSqrDistances.Add(new TargetWithSqrDistancePair(projectile.TargetId, 0));
                }

                // handle hit for every target in damage area
                foreach (TargetWithSqrDistancePair targetWithSqrDistance in _targetsWithSqrDistances)
                {
                    HandleHitShootable(projectile, _context.FieldModel.Shootables[targetWithSqrDistance.TargetId],
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

        private void UpdateHitShootables()
        {
            foreach (KeyValuePair<int, IShootable> shootablePair in _hitShootables)
            {
                if (shootablePair.Value.Health.Value <= 0)
                {
                    _deadBodies.Add(shootablePair.Key, shootablePair.Value);
                    shootablePair.Value.Die();

                    // count kill
                    foreach (int attackingTowerId in _shootablesWithAttackingTowers[shootablePair.Key])
                    {
                        _context.FieldModel.Towers[attackingTowerId].IncreaseScore();
                    }
                }
            }
            
            _hitShootables.Clear();
            _shootablesWithAttackingTowers.Clear();
        }

        private void UpdateDeadBodies()
        {
            foreach (KeyValuePair<int, IShootable> deadBodyPair in _deadBodies)
            {
                if (deadBodyPair.Value.IsCarrion)
                    _carrionBodies.Add(deadBodyPair.Value);
            }

            foreach (IShootable carrionBody in _carrionBodies)
            {
                _deadBodies.Remove(carrionBody.TargetId);
                carrionBody.RemoveBody();
            }
            
            _carrionBodies.Clear();
        }

        private bool Aim(TowerController tower)
        {
            return tower.FindTarget(_targetFinder, _context.FieldModel.MobsManager.MobsByPosition);
        }

        private void Shoot(TowerController tower)
        {
            _context.FieldModel.AddProjectile(tower.Shoot(_context.Factory));
        }

        private void HandleHitShootable(ProjectileController projectile, IShootable hitShootable, float sqrDistance)
        {
            int damage = ComputeDamage(projectile.SpawnTowerId, hitShootable.TargetId,
                projectile.HasSplashDamage, projectile.SplashDamageRadius, projectile.HasProgressiveSplash,
                sqrDistance);
            
            hitShootable.Hurt(damage);
            
            projectile.BaseTowerAttackEffect.ApplyAttack(hitShootable, _context.BuffManager);
            // List<AbstractBuffParameters> buffs = ComputeBuffs(projectile.SpawnTowerId, hitShootable.TargetId);
            // hitShootable.ApplyBuffs(buffs);
            
            if (!_hitShootables.ContainsKey(hitShootable.TargetId))
                _hitShootables.Add(hitShootable.TargetId, hitShootable);
            
            if (!_shootablesWithAttackingTowers.ContainsKey(hitShootable.TargetId))
                _shootablesWithAttackingTowers.Add(hitShootable.TargetId, new List<int>());
            
            _shootablesWithAttackingTowers[hitShootable.TargetId].Add(projectile.SpawnTowerId);
        }

        private int ComputeDamage(int towerId, int targetId,
            bool hasSplashDamage, float splashDamageRadius, bool hasProgressiveSplash, float sqrDistance)
        {
            if (hasSplashDamage)
            {
                float splashDistanceDecreaseCoeff = hasProgressiveSplash
                    ? 1 - sqrDistance / splashDamageRadius
                    : 1;
                return Mathf.CeilToInt(_context.FieldModel.Towers[towerId].CurrentDamage * splashDistanceDecreaseCoeff);
            }
            
            return Mathf.CeilToInt(_context.FieldModel.Towers[towerId].CurrentDamage);
        }

        // private List<AbstractBuffParameters> ComputeBuffs(int towerId, int targetId)
        // {
        //     return _context.FieldModel.Towers[towerId].MobsBuffs;
        // }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _hittingProjectiles.Clear();
            _endSplashingProjectiles.Clear();
            _hitShootables.Clear();
            _shootablesWithAttackingTowers.Clear();
            _targetsWithSqrDistances.Clear();
        }
    }
}