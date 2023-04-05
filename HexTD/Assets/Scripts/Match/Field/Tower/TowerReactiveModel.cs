using Tools;
using UniRx;

namespace Match.Field.Tower
{
    // is used for some special fields, that change from level to level and are used often
    public class TowerReactiveModel : BaseDisposable
    {
        private readonly ReactiveProperty<float> _attackPowerReactiveProperty;
        private readonly ReactiveProperty<float> _attackRadiusReactiveProperty;
        private readonly ReactiveProperty<float> _reloadTimeReactiveProperty;
        private readonly ReactiveProperty<int> _killsCountReactiveProperty;

        public IReadOnlyReactiveProperty<float> AttackPowerReactiveProperty => _attackPowerReactiveProperty;
        public IReadOnlyReactiveProperty<float> AttackRadiusReactiveProperty => _attackRadiusReactiveProperty;
        public IReadOnlyReactiveProperty<float> ReloadTimeReactiveProperty => _reloadTimeReactiveProperty;
        public IReadOnlyReactiveProperty<int> KillsCountReactiveProperty => _killsCountReactiveProperty;

        public TowerReactiveModel()
        {
            _attackPowerReactiveProperty = AddDisposable(new ReactiveProperty<float>());
            _attackRadiusReactiveProperty = AddDisposable(new ReactiveProperty<float>());
            _reloadTimeReactiveProperty = AddDisposable(new ReactiveProperty<float>());
            _killsCountReactiveProperty = AddDisposable(new ReactiveProperty<int>(0));
        }

        public void SetLevel(TowerLevelParams levelParams)
        {
            _attackPowerReactiveProperty.Value = levelParams.LevelRegularParams.Data.AttackPower;
            _attackRadiusReactiveProperty.Value = levelParams.LevelRegularParams.Data.AttackRadius;
            _reloadTimeReactiveProperty.Value = levelParams.LevelRegularParams.Data.ReloadTime;
        }

        public void IncreaseKills()
        {
            _killsCountReactiveProperty.Value++;
        }
    }
}