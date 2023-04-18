using System.Collections.Generic;
using Match.Field.Tower;
using Tools.Interfaces;
using UI.Tools;
using UniRx;

namespace Match.Field.Hand
{
    public class PlayerHandController : BaseDisposable, IOuterLogicUpdatable
    {
        private readonly EnergyCharger _energyCharger;
        private readonly ReactiveCollection<TowerType> _towers;

        private TowerType _chosenTowerType;

        public TowerType ChosenTowerType => _chosenTowerType;
        public bool IsTowerChoice => _chosenTowerType != TowerType.Undefined;
        
        public EnergyCharger EnergyCharger => _energyCharger;
        public IReadOnlyList<TowerType> Towers => _towers;
        public IReadOnlyReactiveCollection<TowerType> ReactiveTowers => _towers;

        public PlayerHandController(TowerType[] towers,
            float energyRestoreDelay, int energyRestoreValue, int maxEnergy)
        {
            _towers = new ReactiveCollection<TowerType>(towers);
            _energyCharger = AddDisposable(new EnergyCharger(energyRestoreDelay, energyRestoreValue, maxEnergy));
            _chosenTowerType = TowerType.Undefined;
        }

        public void AddTower(TowerType towerType)
        {
            _towers.Add(towerType);
        }

        public void RemoveTower(TowerType towerType)
        {
            _towers.Remove(towerType);
        }

        public void SetChosenTower(TowerType towerType)
        {
            _chosenTowerType = towerType;
        }

        public void UseChosenTower(int cost)
        {
            _towers.Remove(_chosenTowerType);
            _energyCharger.SpendEnergy(cost);
            _chosenTowerType = TowerType.Undefined;
        }

        public void AddEnergy(int refundCount)
        {
            _energyCharger.AddEnergy(refundCount);
        }

        public void OuterLogicUpdate(float frameLength)
        {
            _energyCharger.OuterLogicUpdate(frameLength);
        }
    }
}