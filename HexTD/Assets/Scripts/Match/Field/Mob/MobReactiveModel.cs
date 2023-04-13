using System;
using HexSystem;
using Tools;
using UniRx;

namespace Match.Field.Mob
{
    public class MobReactiveModel : BaseDisposable
    {
        private readonly ReactiveProperty<float> _speedReactiveProperty;
        private readonly ReactiveProperty<int> _healthReactiveProperty;
        private Action<MobController, Hex2d> _onHexPositionChange;

        // used only as a reference value
        public IReadOnlyReactiveProperty<float> SpeedReactiveProperty => _speedReactiveProperty;
        // can be changed by buffs with relative value
        public ReactiveProperty<int> HealthReactiveProperty => _healthReactiveProperty;

        public MobReactiveModel(float speed, int health)
        {
            _speedReactiveProperty = AddDisposable(new ReactiveProperty<float>(speed));
            _healthReactiveProperty = AddDisposable(new ReactiveProperty<int>(health));
        }

        public void SubscribeOnHexPositionChange(Action<MobController, Hex2d> actionOnChange)
        {
            _onHexPositionChange += actionOnChange;
        }
        
        public void OnHexPositionChange(MobController mobController, Hex2d newPosition)
        {
            _onHexPositionChange?.Invoke(mobController, newPosition);
        }
    }
}