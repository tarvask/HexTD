using Match.Field.Shooting;

namespace BuffLogic
{
    public class FloatBuffableWithImpactValue : BaseBuffableValue<FloatImpactableBuffableValue>
    {
        public FloatBuffableWithImpactValue(float defaultValue, int targetId, EntityBuffableValueType entityBuffableValueType) 
            : base(new FloatImpactableBuffableValue(defaultValue, targetId, entityBuffableValueType), targetId, entityBuffableValueType)
        {
        }
    }
}