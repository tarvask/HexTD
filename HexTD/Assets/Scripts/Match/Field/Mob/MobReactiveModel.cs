using Tools;
using UniRx;

namespace Match.Field.Mob
{
    public class MobReactiveModel : BaseDisposable
    {
        private readonly ReactiveProperty<float> _speedReactiveProperty;
        private readonly ReactiveProperty<int> _healthReactiveProperty;

        // used only as a reference value
        public IReadOnlyReactiveProperty<float> SpeedReactiveProperty => _speedReactiveProperty;
        // can be changed by buffs with relative value
        public ReactiveProperty<int> HealthReactiveProperty => _healthReactiveProperty;

        public MobReactiveModel(float speed, int health)
        {
            _speedReactiveProperty = AddDisposable(new ReactiveProperty<float>(speed));
            _healthReactiveProperty = AddDisposable(new ReactiveProperty<int>(health));
        }
    }
}