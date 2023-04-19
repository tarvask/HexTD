using Match.Field.Mob;
using Tools;
using UniRx;

namespace Match.Field.Currency
{
    public class CurrencyController : BaseDisposable
    {
        public struct Context
        {
            public int StartCoins { get; }
            public int StartCrystals { get; }

            public ReactiveCommand<MobController> RemoveMobReactiveCommand { get; }
            public ReactiveCommand<int> CrystalCollectedReactiveCommand { get; }

            public Context(
                int startCoins, 
                int startCrystals,
                ReactiveCommand<MobController> removeMobReactiveCommand,
                ReactiveCommand<int> crystalCollectedReactiveCommand)
            {
                StartCoins = startCoins;
                StartCrystals = startCrystals;
                RemoveMobReactiveCommand = removeMobReactiveCommand;
                CrystalCollectedReactiveCommand = crystalCollectedReactiveCommand;
            }
        }

        private readonly Context _context;

        private readonly ReactiveProperty<int> _сoinsCountReactiveProperty;
        private readonly ReactiveProperty<int> _crystalsCountReactiveProperty;

        public IReadOnlyReactiveProperty<int> СoinsCountReactiveProperty => _сoinsCountReactiveProperty;
        public IReadOnlyReactiveProperty<int> CrystalsCountReactiveProperty => _crystalsCountReactiveProperty;

        public CurrencyController(Context context)
        {
            _context = context;
            
            _сoinsCountReactiveProperty = AddDisposable(new ReactiveProperty<int>(_context.StartCoins));
            _crystalsCountReactiveProperty = AddDisposable(new ReactiveProperty<int>(_context.StartCrystals));

            _context.RemoveMobReactiveCommand.Subscribe(RewardForRemovedMob);
            _context.CrystalCollectedReactiveCommand.Subscribe(AddCrystals);
        }

        public bool SpendCoins(int priceInCoins)
        {
            if (priceInCoins <= _сoinsCountReactiveProperty.Value)
            {
                _сoinsCountReactiveProperty.Value -= priceInCoins;
                return true;
            }
            
            return false;
        }

        public void AddCoins(int rewardInCoins)
        {
            _сoinsCountReactiveProperty.Value += rewardInCoins;
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
                AddCoins(mob.RewardInCoins);
        }
    }
}