using BuffLogic;
using UI.Tools;

namespace Match.Field.Shooting
{
    public class BaseReactiveModel : BaseDisposable
    {
        private readonly BaseBuffableValue<float> _damage;
        private readonly BaseBuffableValue<float> _health;

        public IReadonlyBuffableValue<float> Damage => _damage;
        public IReadonlyBuffableValue<float> Health => _health;

        public BaseReactiveModel(float health)
        {
            _damage = AddDisposable(new BaseBuffableValue<float>(1));
            _health = AddDisposable(new BaseBuffableValue<float>(health));
        }

        public void SetHealth(float newHealth)
        {
            _health.Value = newHealth;
        }

        public virtual bool TryGetBuffableValue(EntityBuffableValueType buffableValueType, out IBuffableValue buffableValue)
        {
            switch (buffableValueType)
            {
                case EntityBuffableValueType.Health:
                    buffableValue = _health;
                    return true;
                
                case EntityBuffableValueType.Damage:
                    buffableValue = _damage;
                    return true;
                
                default:
                    buffableValue = null;
                    return false;
            }
        }
    }
}