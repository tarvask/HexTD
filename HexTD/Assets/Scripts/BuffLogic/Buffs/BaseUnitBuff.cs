﻿using System;
using ExitGames.Client.Photon;

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

        public void OuterLogicUpdate(float frameLength)
        {
            if (BuffableValue == null)
                return;

            UpdateBuff(frameLength);

            _isEndConditionDone = ConditionCheck();
        }

        public abstract void MergeBuffs<TBuff>(TBuff buff) where TBuff : IBuff;
        
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
        
        protected abstract void UpdateBuff(float frameLength);
        protected abstract bool ConditionCheck();
        
        public abstract Hashtable ToNetwork();

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