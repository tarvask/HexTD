using Match.Field.Tower.TowerConfigs;
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

        public void SetLevel(TowerConfigNew towerConfig, int level)
        {
            //TODO: sinc value with level
            _attackPowerReactiveProperty.Value = towerConfig.TowerAttackConfig.BaseDamage;
            _attackRadiusReactiveProperty.Value = towerConfig.TowerAttackConfig.AttackRadiusInHex;
            _reloadTimeReactiveProperty.Value = towerConfig.TowerAttackConfig.Cooldown;
        }

        public void IncreaseKills()
        {
            _killsCountReactiveProperty.Value++;
        }
    }
}