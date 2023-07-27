using BuffLogic;
using UI.Tools;

namespace Match.Field.Shooting
{
    public class BaseReactiveModel : BaseDisposable
    {
        private readonly FloatBuffableWithImpactValue _damage;
        private readonly FloatBuffableWithImpactValue _health;

        public FloatBuffableWithImpactValue Damage => _damage;
        public FloatBuffableWithImpactValue Health => _health;

        protected BaseReactiveModel(float health, int targetId)
        {
            _damage = AddDisposable(new FloatBuffableWithImpactValue(1, targetId, EntityBuffableValueType.Damage));
            _health = AddDisposable(new FloatBuffableWithImpactValue(health, targetId, EntityBuffableValueType.MaxHealth));
        }

        public void SetMaxHealth(float newHealth)
        {
            _health.Value.Value = newHealth;
        }

        public void SetHealth(float newHealth)
        {
            _health.Value.SetValue(newHealth);
        }

        public virtual bool TryGetBuffableValue(EntityBuffableValueType buffableValueType, out IBuffableValue buffableValue)
        {
            switch (buffableValueType)
            {
                case EntityBuffableValueType.MaxHealth:
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