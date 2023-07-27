using System;
using System.Collections.Generic;
using Match.Field.Shooting;
using Tools;
using Tools.PriorityTools;
using UniRx;

namespace BuffLogic
{
    public class BaseBuffableValue<TValue> : BaseDisposable, IReadonlyBuffableValue<TValue>
    {
        #region Fields

        private readonly ReactiveProperty<TValue> _value;
        private readonly TValue _defaultValue;
        private IEnumerable<IBuff<TValue>> _buffs;
        private IReadonlyBuffableValue<TValue> readonlyBuffableValueImplementation;

        public TValue Value
        {
            get => _value.Value;
            set => _value.Value = value;
        }
        
        public int TargetId { get; }
        public EntityBuffableValueType EntityBuffableValueType { get; }

        #endregion

        #region BuffMethods
        
        public BaseBuffableValue(TValue defaultValue, int targetId, EntityBuffableValueType entityBuffableValueType)
        {
            TargetId = targetId;
            EntityBuffableValueType = entityBuffableValueType;
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

        protected virtual void ApplyBuffs()
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
        
        public void UpdateAddBuff(IEnumerable<IBuff<TValue>> buffs, IBuff<TValue> addedBuff)
        {
            _buffs = buffs;
            ApplyBuffs();
        }

        public void UpdateRemoveBuffs(IEnumerable<IBuff<TValue>> buffs, IEnumerable<IBuff<TValue>> removedBuffs)
        {
            _buffs = buffs;
            ApplyBuffs();
        }
        
        public void UpdateAddBuff(IEnumerable<IBuff> buffs, IBuff addedBuff)
        {
            throw new NotImplementedException();
        }

        public void UpdateRemoveBuffs(IEnumerable<IBuff> buffs, IEnumerable<IBuff> removedBuffs)
        {
            throw new NotImplementedException();
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