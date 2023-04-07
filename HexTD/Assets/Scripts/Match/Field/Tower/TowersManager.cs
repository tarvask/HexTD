using System.Collections.Generic;
using HexSystem;
using Match.Field.Mob;
using UniRx;

namespace Match.Field.Tower
{
    public class TowersManager
    {        
        public struct Context
        {
            public ReactiveCommand<TowerController> TowerBuiltReactiveCommand { get; }
            public ReactiveCommand<TowerController> TowerPreUpgradedReactiveCommand { get; }
            public ReactiveCommand<TowerController> TowerUpgradedReactiveCommand { get; }
            public ReactiveCommand<TowerController> TowerRemovedReactiveCommand { get; }
            
            public Context(
                ReactiveCommand<TowerController> towerBuiltReactiveCommand,
                ReactiveCommand<TowerController> towerPreUpgradedReactiveCommand,
                ReactiveCommand<TowerController> towerUpgradedReactiveCommand,
                ReactiveCommand<TowerController> towerRemovedReactiveCommand)
            {
                
                TowerBuiltReactiveCommand = towerBuiltReactiveCommand;
                TowerPreUpgradedReactiveCommand = towerPreUpgradedReactiveCommand;
                TowerUpgradedReactiveCommand = towerUpgradedReactiveCommand;
                TowerRemovedReactiveCommand = towerRemovedReactiveCommand;
            }
        }

        private readonly Context _context;
        
        private readonly Dictionary<int, TowerController> _towers;
        private readonly Dictionary<int, TowerController> _towersByPositions;
        
        // towers by ids
        public Dictionary<int, TowerController> Towers => _towers;
        // towers by positions, it's convenient to use in building/upgrading/selling
        public Dictionary<int, TowerController> TowersByPositions => _towersByPositions;

        public TowersManager(int fieldHexGridSize)
        {
            _towers = new Dictionary<int, TowerController>(fieldHexGridSize);
            _towersByPositions = new Dictionary<int, TowerController>(fieldHexGridSize);
        }

        public void AddTower(TowerController tower, Hex2d position)
        {
            _towers.Add(tower.Id, tower);
            _towersByPositions.Add(position.GetHashCode(), tower);

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
            _towers.Remove(removingTower.Id);
            _towersByPositions.Remove(positionHash);

            _context.TowerRemovedReactiveCommand.Execute(removingTower);
            removingTower.Dispose();
        }
    }
}