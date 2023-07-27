using System;
using Match.Field.Shooting;
using UniRx;
using UnityEngine;

namespace BuffLogic
{
    public abstract class BaseImpactableBuffableValue<TValue> : BaseBuffableValue<TValue>, IImpactableBuff<TValue>
    {
        protected ReactiveProperty<TValue> CurrentReactiveValue;
        public TValue CurrentValue => CurrentReactiveValue.Value;

        public BaseImpactableBuffableValue(TValue defaultValue, int targetId, EntityBuffableValueType entityBuffableValueType) 
            : base(defaultValue, targetId, entityBuffableValueType)
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
        public FloatImpactableBuffableValue(float defaultValue, int targetId, EntityBuffableValueType entityBuffableValueType) 
            : base(defaultValue, targetId, entityBuffableValueType)
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