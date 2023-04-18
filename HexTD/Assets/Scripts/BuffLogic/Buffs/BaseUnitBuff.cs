using Match.Field.Mob;
using Match.Field.Shooting;

namespace BuffLogic
{
    public abstract class BaseUnitBuff : IBuff<ITargetable>
    {
        protected ITargetable BuffableValue;
        private bool _isEndConditionDone;

        public int Priority => int.MaxValue;
        public bool IsEndConditionDone => _isEndConditionDone;

        public BaseUnitBuff()
        {
            BuffableValue = null;
            _isEndConditionDone = false;
        }

        public ITargetable ApplyBuff(ITargetable value)
        {
            BuffableValue = value;
            return value;
        }

        public ITargetable RevokeBuff(ITargetable value)
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

        public abstract void MergeBuffs<TBuff>(TBuff buff) where TBuff : IBuff<ITargetable>;
        protected abstract void UpdateBuff();
        protected abstract bool ConditionCheck();
    }
}