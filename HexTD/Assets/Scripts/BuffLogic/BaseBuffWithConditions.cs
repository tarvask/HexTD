﻿using System;
using ExitGames.Client.Photon;
using Match.Serialization;
using UnityEngine;

namespace BuffLogic
{
    public abstract class BaseBuffWithConditions<T>: IBuff<T>
    {
        private const string PriorityParam = "priority";

        [SerializeToNetwork("BuffConditionCollectionType")]
        private readonly EBuffConditionCollectionType _buffConditionCollectionType;
        [SerializableToNetwork("EndConditions")]
        private readonly ABuffConditionsCollection _buffConditionsCollection;

        private Action _onEnd;
        private int _priority;
        
        public int Priority => _priority;
        
        public bool IsEndConditionDone => _buffConditionsCollection.IsEndConditionDone;

        protected BaseBuffWithConditions(EBuffConditionCollectionType buffConditionCollectionType, int priority = int.MaxValue)
        {
            _buffConditionCollectionType = buffConditionCollectionType;
            _buffConditionsCollection = CreateBuffConditionsCollection(_buffConditionCollectionType);
            _priority = priority;
        }

        public abstract T ApplyBuff(T value);
        
        public abstract T RevokeBuff(T value);

        public virtual void OuterLogicUpdate(float frameLength)
        {
            _buffConditionsCollection.Update();
        }

        public abstract void MergeBuffs<TBuff>(TBuff buff) where TBuff : IBuff;

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
        
        public virtual Hashtable ToNetwork()
        {
            Hashtable hashtable = new Hashtable();
            
            hashtable.Add(PhotonEventsConstants.SyncState.PlayerState.Buffs.BuffConditionName, _buffConditionsCollection.ToNetwork());
            hashtable.Add(PriorityParam, _priority);

            return hashtable;
        }

        public object Restore(Hashtable hashtable)
        {
            return null;
        }
    }
}