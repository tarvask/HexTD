namespace BuffLogic
{
    public class BuffConditionComplexCollection : ABuffConditionsCollection
    {
        protected override bool InvokeEndConditions()
        {
            for (var conditionNode = Conditions.First; conditionNode != null; )
            {
                var nextNode = conditionNode.Next;
                if (conditionNode.Value.Invoke())
                {
                    conditionNode.Value.Dispose();
                    Conditions.Remove(conditionNode);
                }
                
                conditionNode = nextNode;
            }

            return Conditions.Count == 0;
        }
    }
}