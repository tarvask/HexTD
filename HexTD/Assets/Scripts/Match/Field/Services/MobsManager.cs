using System.Collections.Generic;
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
            public FieldModel FieldModel { get; }
            public ReactiveCommand<int> AttackCastleByMobReactiveCommand { get; }

            public Context(FieldModel fieldModel,
                ReactiveCommand<int> attackCastleByMobReactiveCommand)
            {
                FieldModel = fieldModel;
                AttackCastleByMobReactiveCommand = attackCastleByMobReactiveCommand;
            }
        }

        private readonly Context _context;
        private readonly List<MobController> _dyingMobs;
        private readonly Dictionary<int, MobController> _deadBodies;
        private readonly List<MobController> _carrionBodies;

        public MobsManager(Context context)
        {
            _context = context;
            
            _dyingMobs = new List<MobController>(WaveMobSpawnerCoordinator.MaxMobsInWave);
            _deadBodies = new Dictionary<int, MobController>(WaveMobSpawnerCoordinator.MaxMobsInWave);
            _carrionBodies = new List<MobController>(WaveMobSpawnerCoordinator.MaxMobsInWave);
        }
        
        public void OuterLogicUpdate(float frameLength)
        {
            UpdateMobsHealth(frameLength);
            UpdateMobsLogicMoving(frameLength);
            UpdateMobsAttacking(frameLength);
        }
        
        public void OuterViewUpdate(float frameLength)
        {
            UpdateMobsVisualMoving(frameLength);
        }

        private void UpdateMobsHealth(float frameLength)
        {
            foreach (KeyValuePair<int, MobController> mobPair in _context.FieldModel.Mobs)
            {
                mobPair.Value.UpdateHealth(frameLength);

                if (mobPair.Value.Health <= 0)
                {
                    _dyingMobs.Add(mobPair.Value);
                }
            }

            // needed to clean up dying mobs from FieldModel.Mobs
            foreach (MobController dyingMob in _dyingMobs)
            {
                _deadBodies.Add(dyingMob.Id, dyingMob);
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
                mob.Dispose();
            }

            _dyingMobs.Clear();
            _carrionBodies.Clear();
        }

        private void UpdateMobsLogicMoving(float frameLength)
        {
            foreach (KeyValuePair<int, MobController> mobPair in _context.FieldModel.Mobs)
            {
                mobPair.Value.LogicMove(frameLength);
            }
        }
        
        private void UpdateMobsVisualMoving(float frameLength)
        {
            foreach (KeyValuePair<int, MobController> mobPair in _context.FieldModel.Mobs)
            {
                mobPair.Value.VisualMove(frameLength);
            }
        }
        
        private void UpdateMobsAttacking(float frameLength)
        {
            foreach (KeyValuePair<int, MobController> mobPair in _context.FieldModel.Mobs)
            {
                if (!mobPair.Value.HasReachedCastle)
                    continue;
                
                mobPair.Value.UpdateTimer(frameLength);
                
                if (mobPair.Value.IsReadyToAttack)
                    _context.AttackCastleByMobReactiveCommand.Execute(mobPair.Value.Attack());
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _dyingMobs.Clear();
            _deadBodies.Clear();
            _carrionBodies.Clear();
        }
    }
}