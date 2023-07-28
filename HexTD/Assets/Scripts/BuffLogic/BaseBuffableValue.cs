using System;
using System.Collections.Generic;
using Match.Field.Shooting;
using Tools;
using UniRx;

namespace BuffLogic
{
    public class BaseBuffableValue<TValue> : BaseDisposable, IReadonlyBuffableValue<TValue>
    {
        #region Fields

        private readonly ReactiveCommand<IBuffableValue> _onDispose;
        private readonly ReactiveProperty<TValue> _value;
        private readonly TValue _defaultValue;
        private IEnumerable<IBuff> _buffs;

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

        public static TValue ApplyBuffs(TValue defaultValue, IEnumerable<IBuff> buffs)
        {
            TValue value = defaultValue;
            
            foreach (var buff in buffs)
                value = ((IBuff<TValue>)buff).ApplyBuff(value);

            return value;
        }

        protected virtual void ApplyBuffs()
        {
            _value.Value = ApplyBuffs(_defaultValue, _buffs);
        }
        
        public void UpdateAddBuff(IEnumerable<IBuff> buffs, IBuff addedBuff)
        {
            _buffs = buffs;
            ApplyBuffs();
        }

        public void UpdateRemoveBuffs(IEnumerable<IBuff> buffs, IEnumerable<IBuff> removedBuffs)
        {
            _buffs = buffs;
            ApplyBuffs();
        }

        public void SubscribeOnDispose(Action<IBuffableValue> onDispose)
        {
            AddDisposable(_onDispose.Subscribe(onDispose));
        }

        public void Subscribe(Action<TValue> onChangeAction)
        {
            AddDisposable(_value.Subscribe(onChangeAction));
        }
        
        public bool HasValue => _value.HasValue;
        
        public IDisposable Subscribe(IObserver<TValue> observer) => _value.Subscribe(observer);
        
        protected override void OnDispose()
        {
            _onDispose.Execute(this);
        }

        #endregion
    }
}