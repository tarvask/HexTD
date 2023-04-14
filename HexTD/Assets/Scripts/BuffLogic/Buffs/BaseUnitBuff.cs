using Match.Field.Mob;
using Match.Field.Shooting;

namespace BuffLogic
{
    public abstract class BaseUnitBuff : IBuff<IShootable>
    {
        protected IShootable BuffableValue;
        private bool _isEndConditionDone;

        public int Priority => int.MaxValue;
        public bool IsEndConditionDone => _isEndConditionDone;

        public BaseUnitBuff()
        {
            BuffableValue = null;
            _isEndConditionDone = false;
        }

        public IShootable ApplyBuff(IShootable value)
        {
            BuffableValue = value;
            return value;
        }

        public IShootable RevokeBuff(IShootable value)
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

        public abstract void MergeBuffs<TBuff>(TBuff buff) where TBuff : IBuff<IShootable>;
        protected abstract void UpdateBuff();
        protected abstract bool ConditionCheck();
    }
}