using ExitGames.Client.Photon;
using Match.Serialization;

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
        
        public static object FromNetwork(Hashtable hashtable)
        {
            BuffConditionComplexCollection buffConditionComplexCollection = new BuffConditionComplexCollection();
            
            foreach (var hashtableTypePair in SerializerToNetwork.IterateSerializedEnumerable(hashtable))
            {
                var condition = SerializerToNetwork.FromNetwork(hashtableTypePair) as IBuffCondition;
                buffConditionComplexCollection.AddCondition(condition);
            }
            
            return buffConditionComplexCollection;
        }
    }
}