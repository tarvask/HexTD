using System;
using System.Collections.Generic;
using HexSystem;
using Match.Field.Shooting;
using Tools;
using Tools.Interfaces;
using UniRx;

namespace Match.Field.Tower
{
    public class TowersManager : BaseDisposable, IOuterLogicUpdatable
    {
        private readonly TowerContainer _towerContainer;
        
        public ReactiveCommand<TowerController> TowerBuiltReactiveCommand { get; }
        public ReactiveCommand<TowerController> TowerPreUpgradedReactiveCommand { get; }
        public ReactiveCommand<TowerController> TowerUpgradedReactiveCommand { get; }
        public ReactiveCommand<TowerController> TowerRemovedReactiveCommand { get; }

        public ITowerContainer TowerContainer => _towerContainer;
        // towers by ids
        public IReadOnlyDictionary<int, TowerController> Towers => _towerContainer.Towers;
        
        public TowersManager(int fieldHexGridSize)
        {
            _towerContainer = new TowerContainer(fieldHexGridSize);
            
            TowerBuiltReactiveCommand = AddDisposable(new ReactiveCommand<TowerController>());
            TowerPreUpgradedReactiveCommand = AddDisposable(new ReactiveCommand<TowerController>());
            TowerUpgradedReactiveCommand = AddDisposable(new ReactiveCommand<TowerController>());
            TowerRemovedReactiveCommand = AddDisposable(new ReactiveCommand<TowerController>());
        }

        public void AddTower(TowerController tower)
        {
            _towerContainer.AddTower(tower);
            TowerBuiltReactiveCommand.Execute(tower);
        }

        public void UpgradeTower(TowerController tower)
        {
            TowerPreUpgradedReactiveCommand.Execute(tower);
            tower.Upgrade();
            TowerUpgradedReactiveCommand.Execute(tower);
        }
        
        public void RemoveTower(TowerController removingTower)
        {
            _towerContainer.RemoveTower(removingTower);
            TowerRemovedReactiveCommand.Execute(removingTower);
            removingTower.Dispose();
        }

        public void OuterLogicUpdate(float frameLength)
        {
            foreach (var tower in TowerContainer.Towers.Values)
            {
                tower.OuterLogicUpdate(frameLength);
            }
        }

        public void Clear()
        {
            _towerContainer.Clear();
        }
    }
}