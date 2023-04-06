using System;
using System.Collections.Generic;
using HexSystem;
using Match.Field.Castle;
using Match.Field.Hexagonal;
using Match.Field.Mob;
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
            public HexagonalFieldController HexagonalFieldController { get; }
            public TowersManager TowersManager { get; }
            public FieldFactory Factory { get; }
            public ReactiveCommand<MobController> RemoveMobReactiveCommand { get; }
            public ReactiveCommand<MobController> MobSpawnedReactiveCommand { get; }
            public ReactiveProperty<bool> HasMobsOnFieldReactiveProperty { get; }

            public Context(HexagonalFieldController hexagonalFieldController,
                TowersManager towersManager,
                FieldFactory factory,
                ReactiveCommand<MobController> removeMobReactiveCommand,
                ReactiveCommand<MobController> mobSpawnedReactiveCommand,
                ReactiveProperty<bool> hasMobsOnFieldReactiveProperty)
            {
                HexagonalFieldController = hexagonalFieldController;
                TowersManager = towersManager;
                Factory = factory;
                
                RemoveMobReactiveCommand = removeMobReactiveCommand;
                MobSpawnedReactiveCommand = mobSpawnedReactiveCommand;
                HasMobsOnFieldReactiveProperty = hasMobsOnFieldReactiveProperty;
            }
        }

        private readonly Context _context;

        // main state data
        private readonly CastleController _castle;
        private readonly Dictionary<int, MobController> _mobs;
        private readonly Dictionary<int, ProjectileController> _projectiles;
        private readonly Dictionary<int, int> _artifactsOnTowers;
        
        // supporting data
        // objects that can be shot
        private Dictionary<int, IShootable> _shootables;

        public int HexGridSize => _context.HexagonalFieldController.HexGridSize;
        
        // towers by ids
        public Dictionary<int, TowerController> Towers => _context.TowersManager.Towers;
        // towers by positions, it's convenient to use in building/upgrading/selling
        public Dictionary<int, TowerController> TowersByPositions => _context.TowersManager.TowersByPositions;

        // castle
        public CastleController Castle => _castle;
        // mobs by ids
        public Dictionary<int, MobController> Mobs => _mobs;
        // projectiles by ids
        public Dictionary<int, ProjectileController> Projectiles => _projectiles;
        public Dictionary<int, IShootable> Shootables => _shootables;

        public FieldModel(Context context)
        {
            _context = context;

            _castle = AddDisposable(_context.Factory.CreateCastle());
            
            _mobs = new Dictionary<int, MobController>(WaveMobSpawnerCoordinator.MaxMobsInWave);
            _projectiles = new Dictionary<int, ProjectileController>(WaveMobSpawnerCoordinator.MaxMobsInWave * 8); // random stuff
            _shootables = new Dictionary<int, IShootable>(WaveMobSpawnerCoordinator.MaxMobsInWave); // + blockers count
            
            _context.RemoveMobReactiveCommand.Subscribe(RemoveMob);
        }

        public void RestoreCells()
        {
            // back to start state
            _context.HexagonalFieldController.Reset();
        }

        public FieldHex GetFieldHex(Hex2d position)
        {
            return _context.HexagonalFieldController[position];
        }

        public bool IsHexWithType(Hex2d position, FieldHexType checkedType)
        {
            FieldHex fieldHex = _context.HexagonalFieldController[position];
            return fieldHex.FieldHexType == checkedType;
        }

        public void AddTower(TowerController tower, Hex2d position)
        {
            if(!_context.HexagonalFieldController.TryAddTower(position))
                return;
            
            _context.TowersManager.AddTower(tower, position);
        }

        public void UpgradeTower(TowerController tower)
        {
            _context.TowersManager.UpgradeTower(tower);
        }

        public void RemoveTower(int positionHash, TowerController removingTower)
        {
            if(!_context.HexagonalFieldController.TryRemoveTower(positionHash))
                return;
            
            
            removingTower.Dispose();
        }

        public void AddMob(MobController mobController)
        {
            _mobs.Add(mobController.Id, mobController);
            _shootables.Add(mobController.TargetId, mobController);
            _context.MobSpawnedReactiveCommand.Execute(mobController);

            if (!_context.HasMobsOnFieldReactiveProperty.Value)
                _context.HasMobsOnFieldReactiveProperty.Value = true;
        }

        private void RemoveMob(MobController mobController)
        {
            _mobs.Remove(mobController.Id);
            _shootables.Remove(mobController.TargetId);
            
            if (_mobs.Count == 0 && _context.HasMobsOnFieldReactiveProperty.Value)
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