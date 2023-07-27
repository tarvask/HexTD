using System;
using System.Collections.Generic;
using Match.Field.Shooting;
using Match.Serialization;

namespace BuffLogic
{
    public interface IBuffableValue
    {
        [SerializeToNetwork("TargetId")] int TargetId { get; }
        [SerializeToNetwork("EntityBuffable")] EntityBuffableValueType EntityBuffableValueType { get; }
        void UpdateAddBuff(IEnumerable<IBuff> buffs, IBuff addedBuff);
        void UpdateRemoveBuffs(IEnumerable<IBuff> buffs, IEnumerable<IBuff> removedBuffs);
    }

    public interface IBuffableValue<TValue> : IBuffableValue
    {
        void UpdateAddBuff(IEnumerable<IBuff<TValue>> buffs, IBuff<TValue> addedBuff);
        void UpdateRemoveBuffs(IEnumerable<IBuff<TValue>> buffs, IEnumerable<IBuff<TValue>> removedBuffs);
    }

    public interface IReadonlyBuffableValue<TValue> : IBuffableValue<TValue>
    {
        TValue Value { get; }
        void Subscribe(Action<TValue> onChange);
    }
    
    public interface IReadonlyImpactableBuff<TValue> : IReadonlyBuffableValue<TValue>
    {
        [SerializeToNetwork("CurrentValue")] TValue CurrentValue { get; }
        void SubscribeOnSetValue(Action<TValue> onChange);
    }

    public interface IImpactableBuff<TValue> : IReadonlyImpactableBuff<TValue>
    {
        void SetValue(TValue newValue);
    }
}