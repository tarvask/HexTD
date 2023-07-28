using ExitGames.Client.Photon;
using Match.Serialization;

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
        
        public static object FromNetwork(Hashtable hashtable)
        {
            BuffConditionOnceCollection buffConditionOnceCollection = new BuffConditionOnceCollection();
            
            foreach (var hashtableTypePair in SerializerToNetwork.IterateSerializedEnumerable(hashtable))
            {
                var condition = SerializerToNetwork.FromNetwork(hashtableTypePair) as IBuffCondition;
                buffConditionOnceCollection.AddCondition(condition);
            }
            
            return buffConditionOnceCollection;
        }
    }
}