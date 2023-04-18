using Match.Field.Mob;
using Tools;
using Tools.Interfaces;
using UniRx;
using UnityEngine;

namespace Match.Field.Currency
{
    public class CurrencyController : BaseDisposable, IOuterLogicUpdatable
    {
        public struct Context
        {
            public int StartSilverCoins { get; }
            public int StartCrystals { get; }
            public float EnergyRestoreDelay { get; }
            public int EnergyRestoreValue { get; }
            public int MaxEnergy { get; }
            
            public ReactiveCommand<MobController> RemoveMobReactiveCommand { get; }
            public ReactiveCommand<int> CrystalCollectedReactiveCommand { get; }
            public ReactiveCommand<float> MatchStartedReactiveCommand { get; }

            public Context(
                int startSilverCoins, 
                int startCrystals,
                float energyRestoreDelay,
                int energyRestoreValue,
                int maxEnergy,
                ReactiveCommand<MobController> removeMobReactiveCommand,
                ReactiveCommand<int> crystalCollectedReactiveCommand,
                ReactiveCommand<float> matchStartedReactiveCommand)
            {
                StartSilverCoins = startSilverCoins;
                StartCrystals = startCrystals;
                EnergyRestoreDelay = energyRestoreDelay;
                EnergyRestoreValue = energyRestoreValue;
                MaxEnergy = maxEnergy;
                RemoveMobReactiveCommand = removeMobReactiveCommand;
                CrystalCollectedReactiveCommand = crystalCollectedReactiveCommand;
                MatchStartedReactiveCommand = matchStartedReactiveCommand;
            }
        }

        private readonly Context _context;

        private readonly ReactiveProperty<int> _goldCoinsCountReactiveProperty;
        private readonly ReactiveProperty<int> _goldCoinsIncomeReactiveProperty;
        private readonly ReactiveProperty<int> _crystalsCountReactiveProperty;
        
        private SimpleStopwatch _energyRestoreStopwatch;

        public IReadOnlyReactiveProperty<int> GoldCoinsCountReactiveProperty => _goldCoinsCountReactiveProperty;
        public IReadOnlyReactiveProperty<int> GoldCoinsIncomeReactiveProperty => _goldCoinsIncomeReactiveProperty;
        public IReadOnlyReactiveProperty<int> CrystalsCountReactiveProperty => _crystalsCountReactiveProperty;

        public CurrencyController(Context context)
        {
            _context = context;
            
            _goldCoinsCountReactiveProperty = AddDisposable(new ReactiveProperty<int>(_context.StartSilverCoins));
            _goldCoinsIncomeReactiveProperty = AddDisposable(new ReactiveProperty<int>(0));
            _crystalsCountReactiveProperty = AddDisposable(new ReactiveProperty<int>(_context.StartCrystals));

            _context.RemoveMobReactiveCommand.Subscribe(RewardForRemovedMob);
            _context.CrystalCollectedReactiveCommand.Subscribe(AddCrystals);
            
            _energyRestoreStopwatch = new SimpleStopwatch(_context.EnergyRestoreDelay);

            AddDisposable(Observable.Merge(
                    _context.MatchStartedReactiveCommand.AsUnitObservable(),
                    _goldCoinsCountReactiveProperty.Where(value => value < _context.MaxEnergy).AsUnitObservable())
                .Subscribe(f => _energyRestoreStopwatch.Start()));

            AddDisposable(_energyRestoreStopwatch
                .Select(cycles => Mathf.Min(cycles * _context.EnergyRestoreValue,
                    _context.MaxEnergy - _goldCoinsCountReactiveProperty.Value))
                .Subscribe(AddSilver));

            AddDisposable(_goldCoinsCountReactiveProperty
                .Where(value => value >= _context.MaxEnergy).AsUnitObservable()
                .Subscribe(value => _energyRestoreStopwatch.Stop()));
        }

        public bool SpendSilver(int priceInSilver)
        {
            if (priceInSilver <= _goldCoinsCountReactiveProperty.Value)
            {
                _goldCoinsCountReactiveProperty.Value -= priceInSilver;
                return true;
            }
            
            return false;
        }

        public void AddSilver(int rewardInSilver)
        {
            _goldCoinsCountReactiveProperty.Value += rewardInSilver;
        }

        public void IncreaseIncome(int incomeIncreaseDelta)
        {
            _goldCoinsIncomeReactiveProperty.Value += incomeIncreaseDelta;
        }

        public bool SpendCrystals(int priceInCrystals)
        {
            if (priceInCrystals <= _crystalsCountReactiveProperty.Value)
            {
                _crystalsCountReactiveProperty.Value -= priceInCrystals;
                return true;
            }
            
            return false;
        }

        public void AddCrystals(int rewardInCrystals)
        {
            _crystalsCountReactiveProperty.Value += rewardInCrystals;
        }

        private void RewardForRemovedMob(MobController mob)
        {
            if (!mob.IsEscaping)
                AddSilver(mob.RewardInSilver);
        }

        public void OuterLogicUpdate(float frameLength)
        {
            _energyRestoreStopwatch.Update(frameLength);
        }
    }
}