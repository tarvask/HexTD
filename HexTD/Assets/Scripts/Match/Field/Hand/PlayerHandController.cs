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

        private TowerType _choiceTowerType;

        public TowerType ChoiceTowerType => _choiceTowerType;
        public bool IsTowerChoice => _choiceTowerType != TowerType.Undefined;
        
        public EnergyCharger EnergyCharger => _energyCharger;
        public IReadOnlyList<TowerType> Towers => _towers;
        public IReadOnlyReactiveCollection<TowerType> ReactiveTowers => _towers;

        public PlayerHandController(TowerType[] towers, float energyRecoverySpeed)
        {
            _towers = new ReactiveCollection<TowerType>(towers);
            _energyCharger = AddDisposable(new EnergyCharger(energyRecoverySpeed));
            _choiceTowerType = TowerType.Undefined;
        }

        public void AddTower(TowerType towerType)
        {
            _towers.Add(towerType);
        }

        public void RemoveTower(TowerType towerType)
        {
            _towers.Remove(towerType);
        }

        public void SetChoiceTower(TowerType towerType)
        {
            _choiceTowerType = towerType;
        }

        public void UseChoiceTower(int cost)
        {
            _towers.Remove(_choiceTowerType);
            _energyCharger.SpendEnergy(cost);
            _choiceTowerType = TowerType.Undefined;
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