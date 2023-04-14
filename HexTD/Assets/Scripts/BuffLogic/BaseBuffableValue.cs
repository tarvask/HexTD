using System;
using System.Collections.Generic;
using Tools;
using Tools.PriorityTools;
using UniRx;

namespace BuffLogic
{
    public class BaseBuffableValue<TValue> : BaseDisposable, IReadonlyBuffableValue<TValue>, IBuffableValue<TValue>
    {
        #region Fields

        private readonly ReactiveProperty<TValue> _value;
        private readonly TValue _defaultValue;
        
        public TValue Value
        {
            get => _value.Value;
            set => _value.Value = value;
        }

        #endregion

        #region BuffMethods
        
        public BaseBuffableValue(TValue defaultValue)
        {
            _defaultValue = defaultValue;
            _value = AddDisposable(new ReactiveProperty<TValue>(defaultValue));
        }

        public void ApplyBuffs(PrioritizeLinkedList<IBuff<TValue>> buffs)
        {
            TValue value = _defaultValue;
            
            foreach (var buff in buffs)
                value = buff.ApplyBuff(value);

            _value.Value = value;
        }

        public void UpdateAddBuff(PrioritizeLinkedList<IBuff<TValue>> buffs, IBuff<TValue> addedBuff)
        {
            ApplyBuffs(buffs);
        }

        public void UpdateRemoveBuffs(PrioritizeLinkedList<IBuff<TValue>> buffs, IEnumerable<IBuff<TValue>> removedBuffs)
        {
            ApplyBuffs(buffs);
        }

        public void Subscribe(Action<TValue> onChangeAction)
        {
            AddDisposable(_value.Subscribe(onChangeAction));
        }

        #endregion
    }
}