using System.Collections.Generic;
using HexSystem;
using Match.Field.Castle;
using Match.Field.Hexagons;
using Match.Field.Mob;
using Match.Field.Services;
using Match.Field.Shooting;
using Match.Field.Tower;
using Match.Wave;
using Tools;
using UniRx;

namespace Match.Field
{
    public class FieldModel : BaseDisposable
    {
        public struct Context
        {
            public FieldHexTypesController FieldHexTypesController { get; }
            public TowersManager TowersManager { get; }
            public MobsManager MobsManager { get; }
            public FieldFactory Factory { get; }
            public ReactiveCommand<MobController> RemoveMobReactiveCommand { get; }
            public ReactiveCommand<MobController> MobSpawnedReactiveCommand { get; }
            public ReactiveProperty<bool> HasMobsOnFieldReactiveProperty { get; }

            public Context(FieldHexTypesController fieldHexTypesController,
                TowersManager towersManager,
                MobsManager mobsManager,
                FieldFactory factory,
                ReactiveCommand<MobController> removeMobReactiveCommand,
                ReactiveCommand<MobController> mobSpawnedReactiveCommand,
                ReactiveProperty<bool> hasMobsOnFieldReactiveProperty)
            {
                FieldHexTypesController = fieldHexTypesController;
                TowersManager = towersManager;
                MobsManager = mobsManager;
                Factory = factory;
                
                RemoveMobReactiveCommand = removeMobReactiveCommand;
                MobSpawnedReactiveCommand = mobSpawnedReactiveCommand;
                HasMobsOnFieldReactiveProperty = hasMobsOnFieldReactiveProperty;
            }
        }

        private readonly Context _context;

        // main state data
        private readonly CastleController _castle;
        private readonly Dictionary<int, ProjectileController> _projectiles;
        private readonly Dictionary<int, int> _artifactsOnTowers;
        
        // supporting data
        // objects that can be shot
        private Dictionary<int, ITargetable> _shootables;

        public int HexGridSize => _context.FieldHexTypesController.HexGridSize;
        
        // towers by ids
        public Dictionary<int, TowerController> Towers => _context.TowersManager.Towers;
        // towers by positions, it's convenient to use in building/upgrading/selling
        public Dictionary<int, TowerController> TowersByPositions => _context.TowersManager.TowersByPositions;

        // castle
        public CastleController Castle => _castle;
        // mobs by ids
        public MobsManager MobsManager => _context.MobsManager;
        // projectiles by ids
        public Dictionary<int, ProjectileController> Projectiles => _projectiles;
        public Dictionary<int, ITargetable> Shootables => _shootables;

        public FieldModel(Context context)
        {
            _context = context;

            _castle = AddDisposable(_context.Factory.CreateCastle());
            
            _projectiles = new Dictionary<int, ProjectileController>(WaveMobSpawnerCoordinator.MaxMobsInWave * 8); // random stuff
            _shootables = new Dictionary<int, ITargetable>(WaveMobSpawnerCoordinator.MaxMobsInWave); // + blockers count
            
            _context.RemoveMobReactiveCommand.Subscribe(RemoveMob);
        }

        public FieldHexType GetFieldHexType(Hex2d position)
        {
            return _context.FieldHexTypesController[position.GetHashCode()];
        }

        public bool IsHexWithType(Hex2d position, FieldHexType checkedType)
        {
            FieldHexType fieldHexType = _context.FieldHexTypesController[position.GetHashCode()];
            return fieldHexType == checkedType;
        }

        public void AddTower(TowerController tower, Hex2d position)
        {
            if(!_context.FieldHexTypesController.TryAddTower(position.GetHashCode()))
                return;
            
            _context.TowersManager.AddTower(tower, position);
        }

        public void UpgradeTower(TowerController tower)
        {
            _context.TowersManager.UpgradeTower(tower);
        }

        public void RemoveTower(int positionHash, TowerController removingTower)
        {
            if(!_context.FieldHexTypesController.TryRemoveTower(positionHash))
                return;
            
            
            removingTower.Dispose();
        }

        public void AddMob(MobController mobController)
        {
            _context.MobsManager.AddMob(mobController);
            _shootables.Add(mobController.TargetId, mobController);
            _context.MobSpawnedReactiveCommand.Execute(mobController);

            if (!_context.HasMobsOnFieldReactiveProperty.Value)
                _context.HasMobsOnFieldReactiveProperty.Value = true;
        }

        private void RemoveMob(MobController mobController)
        {
            _shootables.Remove(mobController.TargetId);
            
            if (_context.MobsManager.MobCount == 0 && _context.HasMobsOnFieldReactiveProperty.Value)
                _context.HasMobsOnFieldReactiveProperty.Value = false;
        }

        public void AddProjectile(ProjectileController projectileController)
        {
            _projectiles.Add(projectileController.Id, projectileController);
        }

        public void RemoveProjectile(int projectileId)
        {
            _projectiles.Remove(projectileId);
        }
    }
}