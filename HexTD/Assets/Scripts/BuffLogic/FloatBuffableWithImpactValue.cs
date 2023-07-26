namespace BuffLogic
{
    public class FloatBuffableWithImpactValue : BaseBuffableValue<FloatImpactableBuffableValue>
    {
        public FloatBuffableWithImpactValue(float defaultValue) : base(new FloatImpactableBuffableValue(defaultValue))
        {
        }
    }
}