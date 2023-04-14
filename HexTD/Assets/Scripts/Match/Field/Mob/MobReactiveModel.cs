using System;
using BuffLogic;
using HexSystem;
using Tools;
using UniRx;

namespace Match.Field.Mob
{
    public class MobReactiveModel : BaseDisposable, IReadonlyMobReactiveModel
    {
        private readonly BaseBuffableValue<float> _speed;
        private readonly BaseBuffableValue<float> _health;
        private Action<MobController, Hex2d> _onHexPositionChange;

        // used only as a reference value
        public IReadonlyBuffableValue<float> Speed => _speed;
        // can be changed by buffs with relative value
        public IReadonlyBuffableValue<float> Health => _health;

        public MobReactiveModel(float speed, float health)
        {
            _speed = AddDisposable(new BaseBuffableValue<float>(speed));
            _health = AddDisposable(new BaseBuffableValue<float>(health));
        }

        public void SetHealth(float newHealth)
        {
            _health.Value = newHealth;
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