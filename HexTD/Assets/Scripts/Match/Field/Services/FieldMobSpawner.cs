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
            public ReactiveCommand<MobConfig> SpawnMobReactiveCommand { get; }

            public Context(FieldModel fieldModel,
                FieldFactory fieldFactory,
                ReactiveCommand<MobConfig> spawnMobReactiveCommand)
            {
                FieldModel = fieldModel;
                FieldFactory = fieldFactory;
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
            _context.FieldModel.AddMob(_context.FieldFactory.CreateMob(mobConfig, Vector2.one * 10));
        }
    }
}