using System;
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
        private readonly TargetContainer _targets;
        private readonly ShooterContainer _shooters;

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
            
            AddDisposable(_context.RemoveMobReactiveCommand.Subscribe(RemoveMob));
            AddDisposable(_context.TowersManager.TowerRemovedReactiveCommand.Subscribe(RemoveTowerFromHex));
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
            if (!_context.FieldHexTypesController.TryAddTower(position.GetHashCode()))
                throw new ArgumentException($"Cannot add tower to hex: hex model already has a tower in {position}");

            _context.TowersManager.AddTower(tower);
        }

        public void UpgradeTower(TowerController tower)
        {
            _context.TowersManager.UpgradeTower(tower);
        }

        public void RemoveTower(TowerController removingTower)
        {
            _context.TowersManager.RemoveTower(removingTower);
        }

        private void RemoveTowerFromHex(TowerController removedTower)
        {
            if (!_context.FieldHexTypesController.TryRemoveTower(removedTower.HexPosition.GetHashCode()))
                throw new ArgumentException($"Cannot remove tower form hex: hex model has no tower in {removedTower.HexPosition}");
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

        public void ClearState()
        {
            // towers
            List<TowerController> towersToRemove = new List<TowerController>(TowersManager.Towers.Values);
            foreach (TowerController tower in towersToRemove)
                RemoveTower(tower);

            towersToRemove.Clear();
            TowersManager.Clear();

            // mobs
            foreach (KeyValuePair<int, MobController> mobPair in MobsManager.Mobs)
                MobsManager.UtiliseMob(mobPair.Value);
            
            MobsManager.Clear();
            
            // projectiles
            foreach (KeyValuePair<int, ProjectileController> projectilePair in Projectiles)
                projectilePair.Value.Dispose();
            
            Projectiles.Clear();
        }
    }
}