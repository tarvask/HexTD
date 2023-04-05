using System;
using System.Collections.Generic;
using Match.Field.Castle;
using Match.Field.Mob;
using Match.Field.Shooting;
using Match.Field.Tower;
using Match.Wave;
using Tools;
using UniRx;
using UnityEngine;

namespace Match.Field
{
    public class FieldModel : BaseDisposable
    {
        public struct Context
        {
            public int FieldWidth { get; }
            public int FieldHeight { get; }
            public FieldCellType[,] Cells { get; }
            public FieldFactory Factory { get; }
            public ReactiveCommand<MobController> RemoveMobReactiveCommand { get; }
            public ReactiveCommand<TowerController> TowerBuiltReactiveCommand { get; }
            public ReactiveCommand<TowerController> TowerPreUpgradedReactiveCommand { get; }
            public ReactiveCommand<TowerController> TowerUpgradedReactiveCommand { get; }
            public ReactiveCommand<TowerController> TowerRemovedReactiveCommand { get; }
            public ReactiveCommand<MobController> MobSpawnedReactiveCommand { get; }
            public ReactiveProperty<bool> HasMobsOnFieldReactiveProperty { get; }

            public Context(int fieldWidth, int fieldHeight, FieldCellType[,] cells,
                FieldFactory factory,
                ReactiveCommand<MobController> removeMobReactiveCommand,
                ReactiveCommand<TowerController> towerBuiltReactiveCommand,
                ReactiveCommand<TowerController> towerPreUpgradedReactiveCommand,
                ReactiveCommand<TowerController> towerUpgradedReactiveCommand,
                ReactiveCommand<TowerController> towerRemovedReactiveCommand,
                ReactiveCommand<MobController> mobSpawnedReactiveCommand,
                ReactiveProperty<bool> hasMobsOnFieldReactiveProperty)
            {
                FieldWidth = fieldWidth;
                FieldHeight = fieldHeight;
                Cells = cells;
                Factory = factory;

                RemoveMobReactiveCommand = removeMobReactiveCommand;
                TowerBuiltReactiveCommand = towerBuiltReactiveCommand;
                TowerPreUpgradedReactiveCommand = towerPreUpgradedReactiveCommand;
                TowerUpgradedReactiveCommand = towerUpgradedReactiveCommand;
                TowerRemovedReactiveCommand = towerRemovedReactiveCommand;
                MobSpawnedReactiveCommand = mobSpawnedReactiveCommand;
                HasMobsOnFieldReactiveProperty = hasMobsOnFieldReactiveProperty;
            }
        }

        private readonly Context _context;

        // main state data
        private readonly FieldCellType[,] _cells;
        private readonly Dictionary<int, TowerController> _towers;
        private readonly Dictionary<int, TowerController> _towersByPositions;
        private readonly CastleController _castle;
        private readonly Dictionary<int, MobController> _mobs;
        private readonly Dictionary<int, ProjectileController> _projectiles;
        private readonly Dictionary<int, int> _artifactsOnTowers;
        
        // supporting data
        // road
        private readonly List<Vector2Int> _roadCells;
        // free, blockers, towers cells
        private readonly List<Vector2Int> _playableCells;
        // objects that can be shot
        private Dictionary<int, IShootable> _shootables;

        public int FieldWidth => _context.FieldWidth;
        public FieldCellType[,] Cells => _cells;
        // towers by ids
        public Dictionary<int, TowerController> Towers => _towers;
        // towers by positions, it's convenient to use in building/upgrading/selling
        public Dictionary<int, TowerController> TowersByPositions => _towersByPositions;

        // castle
        public CastleController Castle => _castle;
        // mobs by ids
        public Dictionary<int, MobController> Mobs => _mobs;
        // projectiles by ids
        public Dictionary<int, ProjectileController> Projectiles => _projectiles;
        public List<Vector2Int> PlayableCells => _playableCells;
        public Dictionary<int, IShootable> Shootables => _shootables;

        public FieldModel(Context context)
        {
            _context = context;

            _cells = new FieldCellType[_context.FieldHeight, _context.FieldWidth];
            Array.Copy(_context.Cells, _cells, _context.FieldHeight * _context.FieldWidth);
            ClearBlockers();

            _roadCells = ExtractRoadCells();
            _playableCells = ExtractPlayableCells();
            
            _towers = new Dictionary<int, TowerController>(_playableCells.Count);
            _towersByPositions = new Dictionary<int, TowerController>(_playableCells.Count);
            _castle = AddDisposable(_context.Factory.CreateCastle());
            
            _mobs = new Dictionary<int, MobController>(WaveMobSpawnerCoordinator.MaxMobsInWave);
            _projectiles = new Dictionary<int, ProjectileController>(WaveMobSpawnerCoordinator.MaxMobsInWave * 8); // random stuff
            _shootables = new Dictionary<int, IShootable>(WaveMobSpawnerCoordinator.MaxMobsInWave); // + blockers count
            
            _context.RemoveMobReactiveCommand.Subscribe(RemoveMob);
        }

        // extractors
        private List<Vector2Int> ExtractRoadCells()
        {
            List<Vector2Int> roadCells = new List<Vector2Int>(_context.FieldWidth * _context.FieldHeight);

            for (int yCell = 0; yCell < _context.FieldHeight; yCell++)
                for (int xCell = 0; xCell < _context.FieldWidth; xCell++)
                    if (IsCellRoad(xCell, yCell))
                        roadCells.Add(new Vector2Int(xCell, yCell));

            return roadCells;
        }
        
        private List<Vector2Int> ExtractPlayableCells()
        {
            List<Vector2Int> playableCells = new List<Vector2Int>(_context.FieldWidth * _context.FieldHeight);

            for (int yCell = 0; yCell < _context.FieldHeight; yCell++)
                for (int xCell = 0; xCell < _context.FieldWidth; xCell++)
                    if (IsCellPlayable(xCell, yCell))
                        playableCells.Add(new Vector2Int(xCell, yCell));
            
            return playableCells;
        }

        // checkers
        private bool IsCellRoad(int xCell, int yCell)
        {
            return _cells[yCell, xCell] == FieldCellType.Road;
        }
        
        private bool IsCellPlayable(int xCell, int yCell)
        {
            return _cells[yCell, xCell] == FieldCellType.Free
                    || _cells[yCell, xCell] == FieldCellType.Blocker
                    || _cells[yCell, xCell] == FieldCellType.Tower;
        }

        public bool IsCellNearRoad(Vector2Int cellPosition)
        {
            int leftRegionBorder = Mathf.Max(0, cellPosition.x - 1);
            int rightRegionBorder = Mathf.Min(_context.FieldWidth - 1, cellPosition.x + 1);
            int bottomRegionBorder = Mathf.Max(0, cellPosition.y - 1);
            int topRegionBorder = Mathf.Min(_context.FieldHeight - 1, cellPosition.y + 1);

            for (int yCellNumber = bottomRegionBorder; yCellNumber <= topRegionBorder; yCellNumber++)
            {
                for (int xCellNumber = leftRegionBorder; xCellNumber <= rightRegionBorder; xCellNumber++)
                {
                    if (_cells[yCellNumber, xCellNumber] == FieldCellType.Road)
                        return true;
                }
            }

            return false;
        }

        private bool IsCellBlockerToDrop(int xCell, int yCell)
        {
            return _cells[yCell, xCell] == FieldCellType.Blocker ||
                   _cells[yCell, xCell] == FieldCellType.PossibleBlocker;
        }

        private void ClearBlockers()
        {
            for (int yCell = 0; yCell < _context.FieldHeight; yCell++)
                for (int xCell = 0; xCell < _context.FieldWidth; xCell++)
                    if (IsCellBlockerToDrop(xCell, yCell))
                        _cells[yCell, xCell] = FieldCellType.Free;
        }

        public void RestoreCells()
        {
            // back to start state
            Array.Copy(_context.Cells, _cells, _context.FieldHeight * _context.FieldWidth);
        }

        public void AddTower(TowerController tower, Vector2Int position)
        {
            _cells[position.y, position.x] = FieldCellType.Tower;
            _towers.Add(tower.Id, tower);
            _towersByPositions.Add(position.GetHashCode(_context.FieldWidth), tower);

            _context.TowerBuiltReactiveCommand.Execute(tower);
        }

        public void UpgradeTower(TowerController tower)
        {
            _context.TowerPreUpgradedReactiveCommand.Execute(tower);
            tower.Upgrade();
            _context.TowerUpgradedReactiveCommand.Execute(tower);
        }

        public void RemoveTower(int positionHash, TowerController removingTower)
        {
            _cells[positionHash / _context.FieldWidth, positionHash % _context.FieldWidth] = FieldCellType.Free;
            _towers.Remove(removingTower.Id);
            _towersByPositions.Remove(positionHash);

            _context.TowerRemovedReactiveCommand.Execute(removingTower);
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