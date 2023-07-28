using System;
using System.Collections.Generic;
using Match.Field.Shooting;

namespace BuffLogic
{
    public interface IBuffableValue
    {
        int TargetId { get; }
        EntityBuffableValueType EntityBuffableValueType { get; }
        void UpdateAddBuff(IEnumerable<IBuff> buffs, IBuff addedBuff);
        void UpdateRemoveBuffs(IEnumerable<IBuff> buffs, IEnumerable<IBuff> removedBuffs);
        void SubscribeOnDispose(Action<IBuffableValue> onDispose);
    }

    public interface IReadonlyBuffableValue<TValue> : IBuffableValue
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