using System.Collections.Generic;
using Match.Field.Mob;
using Match.Field.Shooting;
using Match.Field.VFX;
using Match.Wave;
using Tools;
using Tools.Interfaces;
using UniRx;

namespace Match.Field.Services
{
    public class MobsManager : BaseDisposable, IOuterLogicUpdatable, IOuterViewUpdatable
    {
        public struct Context
        {
            public VfxManager VfxManager { get; }
            public MobsByTowersBlocker MobsByTowersBlocker { get; }
            public bool RemoveMobsOnBossAppearing { get; }
            
            public ReactiveCommand<MobController> AttackTowerByMobReactiveCommand { get; }
            public ReactiveCommand<int> ReachCastleByMobReactiveCommand { get; }
            public ReactiveCommand<MobController> RemoveMobReactiveCommand { get; }
            public ReactiveCommand<MobSpawnParameters> SpawnMobReactiveCommand { get; }

            public Context(
                VfxManager vfxManager,
                MobsByTowersBlocker mobsByTowersBlocker,
                bool removeMobsOnBossAppearing,
                
                ReactiveCommand<MobController> attackTowerByMobReactiveCommand,
                ReactiveCommand<int> reachCastleByMobReactiveCommand,
                ReactiveCommand<MobController> removeMobReactiveCommand,
                ReactiveCommand<MobSpawnParameters> spawnMobReactiveCommand)
            {
                VfxManager = vfxManager;
                MobsByTowersBlocker = mobsByTowersBlocker;
                RemoveMobsOnBossAppearing = removeMobsOnBossAppearing;
                
                AttackTowerByMobReactiveCommand = attackTowerByMobReactiveCommand;
                ReachCastleByMobReactiveCommand = reachCastleByMobReactiveCommand;
                RemoveMobReactiveCommand = removeMobReactiveCommand;
                SpawnMobReactiveCommand = spawnMobReactiveCommand;
            }
        }

        private readonly Context _context;
        private readonly MobsContainer _mobsContainer;
        private readonly List<MobController> _dyingMobs;
        private readonly Dictionary<int, MobController> _deadBodies;
        private readonly List<MobController> _carrionBodies;
        private readonly List<MobController> _escapingMobs;

        public ITypeTargetContainer MobContainer => _mobsContainer;
        public IReadOnlyDictionary<int, MobController> Mobs => _mobsContainer.Mobs;
        public int MobCount => _mobsContainer.Mobs.Count;

        public MobsManager(Context context)
        {
            _context = context;

            _mobsContainer = new MobsContainer();
            _dyingMobs = new List<MobController>(WaveMobSpawnerCoordinator.MaxMobsInWave);
            _deadBodies = new Dictionary<int, MobController>(WaveMobSpawnerCoordinator.MaxMobsInWave);
            _carrionBodies = new List<MobController>(WaveMobSpawnerCoordinator.MaxMobsInWave);
            _escapingMobs = new List<MobController>(WaveMobSpawnerCoordinator.MaxMobsInWave);

            _context.SpawnMobReactiveCommand.Subscribe(CheckForBossSpawn);
        }

        public void AddMob(MobController mobController)
        {
            _mobsContainer.AddMob(mobController);
        }

        public void RemoveMob(MobController mobController)
        {
            _mobsContainer.RemoveMob(mobController);
            _context.RemoveMobReactiveCommand.Execute(mobController);
        }
        
        public void OuterLogicUpdate(float frameLength)
        {
            UpdateMobsHealth(frameLength);
            UpdateMobsLogicMoving(frameLength);
            UpdateMobsAttacking(frameLength);
            UpdateMobsEscaping(frameLength);
        }
        
        public void OuterViewUpdate(float frameLength)
        {
            UpdateMobsVisualMoving(frameLength);
        }

        private void UpdateMobsHealth(float frameLength)
        {
            foreach (KeyValuePair<int, MobController> mobPair in _mobsContainer.Mobs)
            {
                if (mobPair.Value.Health.Value <= 0)
                {
                    _dyingMobs.Add(mobPair.Value);
                }
            }
            
            // needed to clean up dying mobs from FieldModel.Mobs
            foreach (MobController dyingMob in _dyingMobs)
            {
                _deadBodies.Add(dyingMob.Id, dyingMob);
                _mobsContainer.RemoveMob(dyingMob);
                dyingMob.Die();
                _context.RemoveMobReactiveCommand.Execute(dyingMob);
            }

            foreach (KeyValuePair<int, MobController> mobPair in _deadBodies)
            {
                if (mobPair.Value.IsCarrion)
                    _carrionBodies.Add(mobPair.Value);
            }

            // needed to clean up deadBodies after cooldown
            foreach (MobController mob in _carrionBodies)
            {
                _deadBodies.Remove(mob.Id);
                _context.VfxManager.ReleaseVfx(mob);
                mob.Dispose();
            }

            _dyingMobs.Clear();
            _carrionBodies.Clear();
        }

        private void UpdateMobsLogicMoving(float frameLength)
        {
            foreach (KeyValuePair<int, MobController> mobPair in _mobsContainer.Mobs)
            {
                if (mobPair.Value.IsBlocked || mobPair.Value.HasReachedCastle)
                    continue;
            
                // special treatment for attacking towers on the way
                if (_context.MobsByTowersBlocker.TryGetBlockingTowerForMob(mobPair.Value, out int blockerId))
                {
                    mobPair.Value.Block(blockerId);
                    continue;
                }
                
                mobPair.Value.LogicMove(frameLength);
            }
        }
        
        private void UpdateMobsVisualMoving(float frameLength)
        {
            foreach (KeyValuePair<int, MobController> mobPair in _mobsContainer.Mobs)
            {
                mobPair.Value.VisualMove(frameLength);
            }
        }
        
        private void UpdateMobsAttacking(float frameLength)
        {
            foreach (KeyValuePair<int, MobController> mobPair in _mobsContainer.Mobs)
            {
                if (!mobPair.Value.IsBlocked)
                    continue;
                
                mobPair.Value.UpdateTimer(frameLength);
                
                if (mobPair.Value.IsReadyToAttack)
                    _context.AttackTowerByMobReactiveCommand.Execute(mobPair.Value);
            }
        }

        private void UpdateMobsEscaping(float frameLength)
        {
            foreach (KeyValuePair<int, MobController> mobPair in _mobsContainer.Mobs)
            {
                if (mobPair.Value.HasReachedCastle)
                    _escapingMobs.Add(mobPair.Value);
            }

            foreach (MobController escapingMob in _escapingMobs)
            {
                if (!escapingMob.IsEscaping)
                {
                    _context.ReachCastleByMobReactiveCommand.Execute(1); // 1 means that 1 mob escaped
                    escapingMob.Escape();
                    _context.RemoveMobReactiveCommand.Execute(escapingMob);
                }

                if (escapingMob.IsEscaping && escapingMob.IsInSafety)
                {
                    RemoveMob(escapingMob);
                    _context.VfxManager.ReleaseVfx(escapingMob);
                    escapingMob.Dispose();
                }
            }
            
            _escapingMobs.Clear();
        }

        private void CheckForBossSpawn(MobSpawnParameters mobSpawnParameters)
        {
            if (mobSpawnParameters.MobConfig.Parameters.IsBoss && _context.RemoveMobsOnBossAppearing)
            {
                RemoveMobsOnBossAppearing();
            }
        }

        private void RemoveMobsOnBossAppearing()
        {
            foreach (KeyValuePair<int, MobController> mobPair in _mobsContainer.Mobs)
            {
                if (mobPair.Value.IsBoss) continue;
                
                // hurt alive mobs to death
                if (mobPair.Value.Health.Value > 0)
                    mobPair.Value.Hurt(mobPair.Value.Health.Value);
            }
        }

        public void Clear()
        {
            _mobsContainer.Clear();
            _deadBodies.Clear();
            _carrionBodies.Clear();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _deadBodies.Clear();
            _carrionBodies.Clear();
        }
    }
}