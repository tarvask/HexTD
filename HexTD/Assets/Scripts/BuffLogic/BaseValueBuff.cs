using UnityEngine;

namespace BuffLogic
{
    public abstract class BaseValueBuff<T> : BaseBuffWithConditions<T>
    {
        protected float BuffValue;

        protected BaseValueBuff(float buffValue, 
            EBuffConditionCollectionType buffConditionCollectionType = EBuffConditionCollectionType.Once,
            int priority = int.MaxValue) : base(buffConditionCollectionType, priority)
        {
            BuffValue = buffValue;
        }
        
        protected BaseValueBuff(float buffValue, ABuffConditionsCollection buffConditionsCollection, int priority)
            : base(buffConditionsCollection, priority)
        {
            BuffValue = buffValue;
        }
        
        public override void MergeBuffs<TBuff>(TBuff buff)
        {
            var buffTypizied = buff as BaseValueBuff<T>;
            if(buffTypizied == null)
            {
                Debug.LogError("Try to cast buff into strange type!");
                return;
            }
            
            BuffValue = buffTypizied.BuffValue;
            MergeConditions(buffTypizied);
        }
    }
}