using ExitGames.Client.Photon;
using Match.Serialization;

namespace BuffLogic
{
    public class AddFloatValueBuff : BaseValueBuff<float>
    {
        public AddFloatValueBuff(float buffValue) : base(buffValue, EBuffConditionCollectionType.Once, 1)
        {
        }

        public override float ApplyBuff(float value) => value + BuffValue;

        public override float RevokeBuff(float value) => value - BuffValue;
    }
    
    public class MultiFloatValueBuff : BaseValueBuff<float>
    {
        //buffValue - percentage
        public MultiFloatValueBuff(float buffValue) : base(buffValue, EBuffConditionCollectionType.Once, 0)
        {
        }
        
        protected MultiFloatValueBuff(float buffValue, ABuffConditionsCollection buffConditionsCollection, int priority)
            : base(buffValue, buffConditionsCollection, priority)
        {
        }

        public override float ApplyBuff(float value) => value * BuffValue;

        public override float RevokeBuff(float value) => value / BuffValue;
        
        public static object FromNetwork(Hashtable hashtable)
        {
            ABuffConditionsCollection conditionsCollection = SerializerToNetwork.FromNetwork(EndConditions, hashtable) as ABuffConditionsCollection;
            int priority = (int)hashtable[PriorityParam];
            float buffValue = (float)hashtable[nameof(BuffValue)];

            return new MultiFloatValueBuff(buffValue, conditionsCollection, priority);
        }
    }
}