using System;
using UnityEngine;

namespace BuffLogic
{
    public abstract class BaseBuffWithConditions<T>: IBuff<T>
    {
        private readonly ABuffConditionsCollection _buffConditionsCollection;

        private Action _onEnd;
        private int _priority;
        
        public int Priority => _priority;
        
        public bool IsEndConditionDone => _buffConditionsCollection.IsEndConditionDone;

        protected BaseBuffWithConditions(EBuffConditionCollectionType buffConditionCollectionType, int priority = int.MaxValue)
        {
            _buffConditionsCollection = CreateBuffConditionsCollection(buffConditionCollectionType);
            _priority = priority;
        }

        public abstract T ApplyBuff(T value);
        
        public abstract T RevokeBuff(T value);

        public void Update()
        {
            _buffConditionsCollection.Update();
        }

        public abstract void MergeBuffs<TBuff>(TBuff buff) where TBuff : IBuff<T>;

        protected void MergeConditions<TBuff>(TBuff buff) where TBuff : BaseBuffWithConditions<T>
        {
            _buffConditionsCollection.MergeConditions(buff._buffConditionsCollection);
        }

        public void ForceSetPriority(int priority)
        {
            _priority = priority;
        }

        public void AddCondition(IBuffCondition condition)
        {
            _buffConditionsCollection.AddCondition(condition);
        }

        public ABuffConditionsCollection CreateBuffConditionsCollection(EBuffConditionCollectionType buffConditionCollectionType)
        {
            switch (buffConditionCollectionType)
            {
                case  EBuffConditionCollectionType.Once:
                    return new BuffConditionOnceCollection();
                case EBuffConditionCollectionType.Complex:
                    return new BuffConditionComplexCollection();
                
                default:
                    Debug.LogError("Unknown type of BuffConditionsCollection");
                    return new BuffConditionOnceCollection();
            }
        }

        public void SubscribeOnEnd(Action onEnd)
        {
            _onEnd += onEnd;
        }

        public void Dispose()
        {
            _buffConditionsCollection?.Dispose();
            _onEnd.Invoke();
        }
    }
}