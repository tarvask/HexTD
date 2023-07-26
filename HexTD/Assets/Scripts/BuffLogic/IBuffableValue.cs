using System;
using System.Collections.Generic;
using Tools.PriorityTools;
using UniRx;

namespace BuffLogic
{
    public interface IBuffableValue { }
    
    public interface IBuffableValue<TValue> : IBuffableValue, IReadOnlyReactiveProperty<TValue>
    {
        void UpdateAddBuff(PrioritizeLinkedList<IBuff<TValue>> buffs, IBuff<TValue> addedBuff);
        void UpdateRemoveBuffs(PrioritizeLinkedList<IBuff<TValue>> buffs, IEnumerable<IBuff<TValue>> removedBuffs);
    }
    
    public interface IReadonlyBuffableValue<TValue> : IBuffableValue<TValue>
    {
        TValue Value { get; }
        void Subscribe(Action<TValue> onChange);
    }
    
    public interface IReadonlyImpactableBuff<TValue> : IReadonlyBuffableValue<TValue>
    {
        TValue CurrentValue { get; }
        void SubscribeOnSetValue(Action<TValue> onChange);
    }

    public interface IImpactableBuff<TValue> : IReadonlyImpactableBuff<TValue>
    {
        void SetValue(TValue newValue);
    }
}