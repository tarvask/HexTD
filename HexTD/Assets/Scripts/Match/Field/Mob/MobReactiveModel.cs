using System;
using BuffLogic;
using HexSystem;
using Match.Field.Shooting;

namespace Match.Field.Mob
{
    public class MobReactiveModel : BaseReactiveModel
    {
        private readonly FloatBuffableWithImpactValue _speed;
        private Action<MobController, Hex2d> _onHexPositionChange;

        public FloatBuffableWithImpactValue Speed => _speed;

        public MobReactiveModel(float speed, float health) : base(health)
        {
            _speed = AddDisposable(new FloatBuffableWithImpactValue(speed));
        }

        public void SubscribeOnHexPositionChange(Action<MobController, Hex2d> actionOnChange)
        {
            _onHexPositionChange += actionOnChange;
        }

        public void UnsubscribeOnHexPositionChange(Action<MobController, Hex2d> actionOnChange)
        {
            _onHexPositionChange -= actionOnChange;
        }
        
        public void OnHexPositionChange(MobController mobController, Hex2d newPosition)
        {
            _onHexPositionChange?.Invoke(mobController, newPosition);
        }

        public override bool TryGetBuffableValue(EntityBuffableValueType buffableValueType, out IBuffableValue buffableValue)
        {
            if (base.TryGetBuffableValue(buffableValueType, out buffableValue))
                return true;

            switch (buffableValueType)
            {
                case EntityBuffableValueType.Speed:
                    buffableValue = _speed;
                    return true;
            }

            return false;
        }
    }
}