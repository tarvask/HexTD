using System;
using UniRx;
using UnityEngine;

namespace BuffLogic
{
    public abstract class BaseImpactableBuffableValue<TValue> : BaseBuffableValue<TValue>, IImpactableBuff<TValue>
    {
        protected ReactiveProperty<TValue> CurrentReactiveValue;
        public TValue CurrentValue => CurrentReactiveValue.Value;

        protected BaseImpactableBuffableValue(TValue defaultValue) : base(defaultValue)
        {
            CurrentReactiveValue = AddDisposable(new ReactiveProperty<TValue>(defaultValue));
        }

        public void SubscribeOnSetValue(Action<TValue> onChange)
        {
            AddDisposable(CurrentReactiveValue.Subscribe(onChange));
        }

        public abstract void SetValue(TValue newValue);
    }

    public class FloatImpactableBuffableValue : BaseImpactableBuffableValue<float>
    {
        public FloatImpactableBuffableValue(float defaultValue) : base(defaultValue)
        {
            
        }

        public override void SetValue(float newValue)
        {
            CurrentReactiveValue.Value = Mathf.Clamp(newValue, 0, Value);
        }

        protected override void ApplyBuffs()
        {
            base.ApplyBuffs();
            SetValue(CurrentReactiveValue.Value);
        }
    }
}