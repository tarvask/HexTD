using Tools;
using Tools.Interfaces;
using UniRx;

namespace Match.Field.Hand
{
    public class EnergyCharger : BaseDisposable, IOuterLogicUpdatable
    {
        private readonly int _energyStartCount;
        private readonly float _energyRestoreDelay;
        private readonly int _energyRestoreValue;
        private readonly int _maxEnergy;
        private readonly IReactiveProperty<float> _energyRecoveryAccumulator;
        private readonly IReactiveProperty<int> _currentEnergyCount;

        public IReadOnlyReactiveProperty<int> CurrentEnergyCount => _currentEnergyCount;

        public EnergyCharger(int energyStartCount, float energyRestoreDelay, int energyRestoreValue, int maxEnergy)
        {
            _energyStartCount = energyStartCount;
            _energyRestoreDelay = energyRestoreDelay;
            _energyRestoreValue = energyRestoreValue;
            _maxEnergy = maxEnergy;
            _currentEnergyCount = AddDisposable(new IntReactiveProperty(_energyStartCount));
            _energyRecoveryAccumulator = AddDisposable(new FloatReactiveProperty(0f));
        }

        public void SpendEnergy(int energyCost)
        {
            _currentEnergyCount.Value -= energyCost;
        }

        public void AddEnergy(int energyCost)
        {
            _currentEnergyCount.Value += energyCost;
        }

        public void OuterLogicUpdate(float frameLength)
        {
            _energyRecoveryAccumulator.Value += frameLength;
            
            if (_energyRecoveryAccumulator.Value >= _energyRestoreDelay)
            {
                _energyRecoveryAccumulator.Value -= _energyRestoreDelay;
                _currentEnergyCount.Value += _energyRestoreValue;

                // if (_currentEnergyCount.Value >= _maxEnergy)
                //     _currentEnergyCount.Value = _maxEnergy;
            }
        }
    }
}