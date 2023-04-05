using Match.Field.Mob;
using Tools;
using UniRx;
using UnityEngine;

namespace Match.Field.Services
{
    public class FieldMobSpawner : BaseDisposable
    {
        public struct Context
        {
            public FieldModel FieldModel { get; }
            public FieldFactory FieldFactory { get; }
            public Vector3[] WayPoints { get; }
            public ReactiveCommand<MobConfig> SpawnMobReactiveCommand { get; }

            public Context(FieldModel fieldModel,
                FieldFactory fieldFactory,
                Vector3[] wayPoints,
                ReactiveCommand<MobConfig> spawnMobReactiveCommand)
            {
                FieldModel = fieldModel;
                FieldFactory = fieldFactory;
                WayPoints = wayPoints;
                SpawnMobReactiveCommand = spawnMobReactiveCommand;
            }
        }
        
        private readonly Context _context;

        public FieldMobSpawner(Context context)
        {
            _context = context;
            
            _context.SpawnMobReactiveCommand.Subscribe(Spawn);
        }
        
        private void Spawn(MobConfig mobConfig)
        {
            _context.FieldModel.AddMob(_context.FieldFactory.CreateMob(mobConfig, _context.WayPoints[0], _context.WayPoints));
        }
    }
}