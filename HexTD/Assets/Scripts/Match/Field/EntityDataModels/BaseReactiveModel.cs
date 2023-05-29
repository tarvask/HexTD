using BuffLogic;
using UI.Tools;
using UniRx;

namespace Match.Field.Shooting
{
    public class BaseReactiveModel : BaseDisposable
    {
        private readonly BaseBuffableValue<float> _damage;
        private readonly BaseBuffableValue<float> _maxHealth;
        private readonly ReactiveProperty<float> _health;

        public IReadonlyBuffableValue<float> Damage => _damage;
        public IReadonlyBuffableValue<float> MaxHealth => _maxHealth;
        public IReadOnlyReactiveProperty<float> Health => _health;

        public BaseReactiveModel(float health)
        {
            _damage = AddDisposable(new BaseBuffableValue<float>(1));
            _maxHealth = AddDisposable(new BaseBuffableValue<float>(health));
            _health = AddDisposable(new ReactiveProperty<float>(health));
        }

        public void SetMaxHealth(float newHealth)
        {
            _maxHealth.Value = newHealth;
        }

        public void SetHealth(float newHealth)
        {
            _health.Value = newHealth;
        }

        public virtual bool TryGetBuffableValue(EntityBuffableValueType buffableValueType, out IBuffableValue<float> buffableValue)
        {
            switch (buffableValueType)
            {
                case EntityBuffableValueType.MaxHealth:
                    buffableValue = _maxHealth;
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