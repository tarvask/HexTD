using System.Collections.Generic;
using Tools.PriorityTools;

namespace BuffLogic
{
    public interface IBuffableValue { }
    
    public interface IBuffableValue<TValue> : IBuffableValue
    {
        void UpdateAddBuff(PrioritizeLinkedList<IBuff<TValue>> buffs, IBuff<TValue> addedBuff);
        void UpdateRemoveBuffs(PrioritizeLinkedList<IBuff<TValue>> buffs, IEnumerable<IBuff<TValue>> removedBuffs);
    }
    
    public interface IReadonlyBuffableValue<TValue> : IBuffableValue<TValue>
    {
        TValue Value { get; }
    }
}