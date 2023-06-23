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
        private IEnumerable<IBuff<TValue>> _buffs;
        
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

        public static TValue ApplyBuffs(TValue defaultValue, IEnumerable<IBuff<TValue>> buffs)
        {
            TValue value = defaultValue;
            
            foreach (var buff in buffs)
                value = buff.ApplyBuff(value);

            return value;
        }

        public TValue CopyValue(TValue defaultValue)
        {
            if (_buffs == null)
                return defaultValue;
            
            return ApplyBuffs(defaultValue, _buffs);
        }

        private void ApplyBuffs()
        {
            _value.Value = ApplyBuffs(_defaultValue, _buffs);
        }

        public void UpdateAddBuff(PrioritizeLinkedList<IBuff<TValue>> buffs, IBuff<TValue> addedBuff)
        {
            _buffs = buffs;
            ApplyBuffs();
        }

        public void UpdateRemoveBuffs(PrioritizeLinkedList<IBuff<TValue>> buffs, IEnumerable<IBuff<TValue>> removedBuffs)
        {
            _buffs = buffs;
            ApplyBuffs();
        }

        public void Subscribe(Action<TValue> onChangeAction)
        {
            AddDisposable(_value.Subscribe(onChangeAction));
        }

        #endregion

        public bool HasValue => _value.HasValue;
        
        public IDisposable Subscribe(IObserver<TValue> observer) => _value.Subscribe(observer);
    }
}