using Match.Field.Tower.TowerConfigs;
using Tools;
using UniRx;

namespace Match.Field.Tower
{
    // is used for some special fields, that change from level to level and are used often
    public class TowerReactiveModel : BaseDisposable
    {
        private readonly ReactiveProperty<int> _killsCountReactiveProperty;

        public IReadOnlyReactiveProperty<int> KillsCountReactiveProperty => _killsCountReactiveProperty;

        public TowerReactiveModel()
        {
            _killsCountReactiveProperty = AddDisposable(new ReactiveProperty<int>(0));
        }

        public void IncreaseKills()
        {
            _killsCountReactiveProperty.Value++;
        }
    }
}