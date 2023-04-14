namespace BuffLogic
{
    public class BuffConditionOnceCollection : ABuffConditionsCollection
    {
        protected override bool InvokeEndConditions()
        {
            foreach (var condition in Conditions)
            {
                if (condition.Invoke())
                    return true;
            }

            return false;
        }
    }
}