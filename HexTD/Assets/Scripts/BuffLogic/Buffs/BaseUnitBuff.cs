using System;
using Match.Field.Shooting;

namespace BuffLogic
{
    public abstract class BaseUnitBuff : IBuff<ITarget>
    {
        protected ITarget BuffableValue;

        private Action _onEnd;
        private bool _isEndConditionDone;

        public int Priority => int.MaxValue;
        public bool IsEndConditionDone => _isEndConditionDone;

        public BaseUnitBuff()
        {
            BuffableValue = null;
            _isEndConditionDone = false;
        }

        public ITarget ApplyBuff(ITarget value)
        {
            BuffableValue = value;
            return value;
        }

        public ITarget RevokeBuff(ITarget value)
        {
            BuffableValue = null;
            return value;
        }

        public void Update()
        {
            if(BuffableValue == null)
                return;

            UpdateBuff();

            _isEndConditionDone = ConditionCheck();
        }

        public abstract void MergeBuffs<TBuff>(TBuff buff) where TBuff : IBuff<ITarget>;
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