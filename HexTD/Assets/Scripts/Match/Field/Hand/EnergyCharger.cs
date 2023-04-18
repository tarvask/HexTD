using Tools;
using Tools.Interfaces;
using UniRx;

namespace Match.Field.Hand
{
    public class EnergyCharger : BaseDisposable, IOuterLogicUpdatable
    {
        private readonly float _energyRecoverySpeed;
        private IReactiveProperty<float> _energyRecoveryAccumulator;
        private IReactiveProperty<int> _currentEnergyCount;

        public float EnergyRecoveryFactor => _energyRecoveryAccumulator.Value / _energyRecoverySpeed;
        public IReadOnlyReactiveProperty<float> EnergyRecoveryAccumulator => _energyRecoveryAccumulator;
        public IReadOnlyReactiveProperty<int> CurrentEnergyCount => _currentEnergyCount;

        public EnergyCharger(float energyRecoverySpeed)
        {
            _energyRecoverySpeed = energyRecoverySpeed;
            _currentEnergyCount = AddDisposable(new IntReactiveProperty(0));
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
            if (_energyRecoveryAccumulator.Value >= _energyRecoverySpeed)
            {
                _energyRecoveryAccumulator.Value -= _energyRecoverySpeed;
                _currentEnergyCount.Value++;
            }
        }
    }
}