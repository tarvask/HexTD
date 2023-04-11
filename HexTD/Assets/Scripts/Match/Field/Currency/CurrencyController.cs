using Match.Field.Mob;
using Tools;
using UniRx;

namespace Match.Field.Currency
{
    public class CurrencyController : BaseDisposable
    {
        public struct Context
        {
            public int StartSilverCoins { get; }
            public int StartCrystals { get; }
            
            public ReactiveCommand<MobController> RemoveMobReactiveCommand { get; }
            public ReactiveCommand<int> CrystalCollectedReactiveCommand { get; }

            public Context(int startSilverCoins, int startCrystals,
                ReactiveCommand<MobController> removeMobReactiveCommand,
                ReactiveCommand<int> crystalCollectedReactiveCommand)
            {
                StartSilverCoins = startSilverCoins;
                StartCrystals = startCrystals;
                RemoveMobReactiveCommand = removeMobReactiveCommand;
                CrystalCollectedReactiveCommand = crystalCollectedReactiveCommand;
            }
        }

        private readonly Context _context;

        private readonly ReactiveProperty<int> _goldCoinsCountReactiveProperty;
        private readonly ReactiveProperty<int> _goldCoinsIncomeReactiveProperty;
        private readonly ReactiveProperty<int> _crystalsCountReactiveProperty;

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
    }
}