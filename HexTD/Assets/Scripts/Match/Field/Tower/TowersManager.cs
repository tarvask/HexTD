using System.Collections.Generic;
using HexSystem;
using Match.Field.Mob;
using Tools;
using UniRx;

namespace Match.Field.Tower
{
    public class TowersManager : BaseDisposable
    {        
        public ReactiveCommand<TowerController> TowerBuiltReactiveCommand { get; }
        public ReactiveCommand<TowerController> TowerPreUpgradedReactiveCommand { get; }
        public ReactiveCommand<TowerController> TowerUpgradedReactiveCommand { get; }
        public ReactiveCommand<TowerController> TowerRemovedReactiveCommand { get; }

        private readonly Dictionary<int, TowerController> _towers;
        private readonly Dictionary<int, TowerController> _towersByPositions;
        
        // towers by ids
        public Dictionary<int, TowerController> Towers => _towers;
        // towers by positions, it's convenient to use in building/upgrading/selling
        public Dictionary<int, TowerController> TowersByPositions => _towersByPositions;

        public TowersManager(int fieldHexGridSize)
        {
            TowerBuiltReactiveCommand = AddDisposable(new ReactiveCommand<TowerController>());
            TowerPreUpgradedReactiveCommand = AddDisposable(new ReactiveCommand<TowerController>());
            TowerUpgradedReactiveCommand = AddDisposable(new ReactiveCommand<TowerController>());
            TowerRemovedReactiveCommand = AddDisposable(new ReactiveCommand<TowerController>());
            
            _towers = new Dictionary<int, TowerController>(fieldHexGridSize);
            _towersByPositions = new Dictionary<int, TowerController>(fieldHexGridSize);
        }

        public void AddTower(TowerController tower, Hex2d position)
        {
            _towers.Add(tower.Id, tower);
            _towersByPositions.Add(position.GetHashCode(), tower);

            TowerBuiltReactiveCommand.Execute(tower);
        }

        public void UpgradeTower(TowerController tower)
        {
            TowerPreUpgradedReactiveCommand.Execute(tower);
            tower.Upgrade();
            TowerUpgradedReactiveCommand.Execute(tower);
        }
        
        public void RemoveTower(int positionHash, TowerController removingTower)
        {
            _towers.Remove(removingTower.Id);
            _towersByPositions.Remove(positionHash);

            TowerRemovedReactiveCommand.Execute(removingTower);
            removingTower.Dispose();
        }
    }
}