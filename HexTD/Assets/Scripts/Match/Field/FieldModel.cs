using System.Collections.Generic;
using HexSystem;
using Match.Field.Castle;
using Match.Field.Hexagons;
using Match.Field.Mob;
using Match.Field.Services;
using Match.Field.Shooting;
using Match.Field.Tower;
using Match.Field.VFX;
using Match.Wave;
using Tools;
using UniRx;

namespace Match.Field
{
    public class FieldModel : BaseDisposable
    {
        public struct Context
        {
            public IHexPositionConversionService HexPositionConversionService { get; }
            public FieldHexTypesController FieldHexTypesController { get; }
            public TowersManager TowersManager { get; }
            public MobsManager MobsManager { get; }
            public FieldFactory Factory { get; }
            public ReactiveCommand<MobController> RemoveMobReactiveCommand { get; }
            public ReactiveCommand<MobController> MobSpawnedReactiveCommand { get; }
            public ReactiveProperty<bool> HasMobsOnFieldReactiveProperty { get; }

            public Context(IHexPositionConversionService hexPositionConversionService,
                FieldHexTypesController fieldHexTypesController,
                TowersManager towersManager,
                MobsManager mobsManager,
                FieldFactory factory,
                ReactiveCommand<MobController> removeMobReactiveCommand,
                ReactiveCommand<MobController> mobSpawnedReactiveCommand,
                ReactiveProperty<bool> hasMobsOnFieldReactiveProperty)
            {
                HexPositionConversionService = hexPositionConversionService;
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
        private TargetContainer _targets;
        private ShooterContainer _shooters;

        public int HexGridSize => _context.FieldHexTypesController.HexGridSize;
        
        // towers by ids
        public TowersManager TowersManager => _context.TowersManager;

        // castle
        public CastleController Castle => _castle;
        // mobs by ids
        public MobsManager MobsManager => _context.MobsManager;
        // projectiles by ids
        public Dictionary<int, ProjectileController> Projectiles => _projectiles;
        public TargetContainer Targets => _targets;
        public ShooterContainer Shooters => _shooters;
        public IHexPositionConversionService HexPositionConversionService => _context.HexPositionConversionService;

        public FieldModel(Context context)
        {
            _context = context;

            _castle = AddDisposable(_context.Factory.CreateCastle());
            
            _projectiles = new Dictionary<int, ProjectileController>(WaveMobSpawnerCoordinator.MaxMobsInWave * 8); // random stuff
            _targets = new TargetContainer(MobsManager.MobContainer, TowersManager.TowerContainer);
            _shooters = new ShooterContainer(_context.TowersManager.TowerContainer); 
            
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

            _context.TowersManager.AddTower(tower);
        }

        public void UpgradeTower(TowerController tower)
        {
            _context.TowersManager.UpgradeTower(tower);
        }

        public void RemoveTower(int positionHash, TowerController removingTower)
        {
            if(!_context.FieldHexTypesController.TryRemoveTower(positionHash))
                return;

            _context.TowersManager.RemoveTower(removingTower);
        }

        public void AddMob(MobController mobController)
        {
            _context.MobsManager.AddMob(mobController);
            _context.MobSpawnedReactiveCommand.Execute(mobController);

            if (!_context.HasMobsOnFieldReactiveProperty.Value)
                _context.HasMobsOnFieldReactiveProperty.Value = true;
        }

        private void RemoveMob(MobController mobController)
        {
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