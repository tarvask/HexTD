using System;
using System.Collections.Generic;
using Tools.PriorityTools;

namespace BuffLogic
{
    public class EmptyUnitBuffableValue<TValue> : IBuffableValue<TValue>
    {
        public void UpdateAddBuff(PrioritizeLinkedList<IBuff<TValue>> buffs, IBuff<TValue> addedBuff)
        {
            
        }

        public void UpdateRemoveBuffs(PrioritizeLinkedList<IBuff<TValue>> buffs, IEnumerable<IBuff<TValue>> removedBuffs)
        {
            
        }

        public TValue Value => throw new NotImplementedException();
        public bool HasValue => throw new NotImplementedException();
        public IDisposable Subscribe(IObserver<TValue> observer) => throw new NotImplementedException();
    }
}