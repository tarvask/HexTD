using System;
using Match.Field.Shooting;

namespace BuffLogic
{
    public abstract class BaseUnitBuff : IBuff<FloatImpactableBuffableValue>
    {
        protected FloatImpactableBuffableValue BuffableValue;

        private Action _onEnd;
        private bool _isEndConditionDone;

        public virtual int Priority => int.MaxValue;
        public bool IsEndConditionDone => _isEndConditionDone;

        protected BaseUnitBuff()
        {
            BuffableValue = null;
            _isEndConditionDone = false;
        }

        public void Update()
        {
            if(BuffableValue == null)
                return;

            UpdateBuff();

            _isEndConditionDone = ConditionCheck();
        }

        public abstract void MergeBuffs<TBuff>(TBuff buff) where TBuff : IBuff<FloatImpactableBuffableValue>;
        
        public FloatImpactableBuffableValue ApplyBuff(FloatImpactableBuffableValue value)
        {
            BuffableValue = value;
            return value;
        }

        public FloatImpactableBuffableValue RevokeBuff(FloatImpactableBuffableValue value)
        {
            BuffableValue = null;
            return value;
        }
        
        protected abstract void UpdateBuff();
        protected abstract bool ConditionCheck();

        public void SubscribeOnEnd(Action onEnd)
        {
            _onEnd += onEnd;
        }

        public void Dispose()
        {
            _onEnd.Invoke();
        }
    }
}