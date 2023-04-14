using System.Collections.Generic;
using Match.Field.Hexagons;
using Match.Field.Mob;
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
            public IHexPositionConversionService HexPositionConversionService { get; }
            public ReactiveCommand<int> AttackCastleByMobReactiveCommand { get; }

            public Context(IHexPositionConversionService hexPositionConversionService,
                ReactiveCommand<int> attackCastleByMobReactiveCommand)
            {
                HexPositionConversionService = hexPositionConversionService;
                AttackCastleByMobReactiveCommand = attackCastleByMobReactiveCommand;
            }
        }

        private readonly Context _context;
        private readonly MobsContainer _mobsContainer;
        private readonly List<MobController> _dyingMobs;
        private readonly Dictionary<int, MobController> _deadBodies;
        private readonly List<MobController> _carrionBodies;
        private readonly List<MobController> _escapingMobs;

        public IReadOnlyDictionary<int, MobController> Mobs => _mobsContainer.Mobs;
        public IReadOnlyDictionary<int, List<MobController>> MobsByPosition => _mobsContainer.MobsByPosition;
        public int MobCount => _mobsContainer.Mobs.Count;

        public MobsManager(Context context)
        {
            _context = context;

            _mobsContainer = new MobsContainer(_context.HexPositionConversionService);
            _dyingMobs = new List<MobController>(WaveMobSpawnerCoordinator.MaxMobsInWave);
            _deadBodies = new Dictionary<int, MobController>(WaveMobSpawnerCoordinator.MaxMobsInWave);
            _carrionBodies = new List<MobController>(WaveMobSpawnerCoordinator.MaxMobsInWave);
            _escapingMobs = new List<MobController>(WaveMobSpawnerCoordinator.MaxMobsInWave);
        }

        public void AddMob(MobController mobController)
        {
            _mobsContainer.AddMob(mobController);
        }

        public void RemoveMob(MobController mobController)
        {
            _mobsContainer.RemoveMob(mobController);
        }
        
        public void OuterLogicUpdate(float frameLength)
        {
            UpdateMobsHealth(frameLength);
            UpdateMobsLogicMoving(frameLength);
            //UpdateMobsAttacking(frameLength);
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
                _mobsContainer.RemoveMob(mob);
                mob.Dispose();
            }

            _dyingMobs.Clear();
            _carrionBodies.Clear();
        }

        private void UpdateMobsLogicMoving(float frameLength)
        {
            _mobsContainer.UpdateMobsLogicMoving(frameLength);
        }
        
        private void UpdateMobsVisualMoving(float frameLength)
        {
            foreach (KeyValuePair<int, MobController> mobPair in _mobsContainer.Mobs)
            {
                mobPair.Value.VisualMove(frameLength);
            }
        }
        
        // private void UpdateMobsAttacking(float frameLength)
        // {
        //     foreach (KeyValuePair<int, MobController> mobPair in _mobsContainer.Mobs)
        //     {
        //         if (!mobPair.Value.HasReachedCastle)
        //             continue;
        //         
        //         mobPair.Value.UpdateTimer(frameLength);
        //         
        //         if (mobPair.Value.IsReadyToAttack)
        //             _context.AttackCastleByMobReactiveCommand.Execute(mobPair.Value.Attack());
        //     }
        // }

        private void UpdateMobsEscaping(float frameLength)
        {
            foreach (KeyValuePair<int, MobController> mobPair in _mobsContainer.Mobs)
            {
                if (mobPair.Value.HasReachedCastle)
                    _escapingMobs.Add(mobPair.Value);
            }

            foreach (MobController mob in _escapingMobs)
            {
                if (!mob.IsEscaping)
                {
                    _context.AttackCastleByMobReactiveCommand.Execute(1); // 1 means that 1 mob escaped
                    mob.Escape();
                }

                if (mob.IsEscaping && mob.IsInSafety)
                {
                    RemoveMob(mob);
                    mob.Dispose();
                }
            }
            
            _escapingMobs.Clear();
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