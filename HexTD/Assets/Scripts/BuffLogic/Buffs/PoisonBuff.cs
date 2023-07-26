using UnityEngine;

namespace BuffLogic
{
    public class PoisonBuff : BaseUnitBuff
    {
        private float _damageCapacity;
        private float _damagePerDelay;
        private float _damageDelay;
        
        private float _delayAccumulator;
        private float _damageImpact;
        
        public PoisonBuff(float damageCapacity, float damagePerDelay, float damageDelay = 4f/60f)
        {
            _damageCapacity = damageCapacity;
            _damagePerDelay = damagePerDelay;
            _damageDelay = damageDelay;
            
            _delayAccumulator = 0f;
            _damageImpact = 0f;
        }

        protected override void UpdateBuff()
        {
            _delayAccumulator += Time.deltaTime;

            while (_delayAccumulator >= _damageDelay)
            {
                BuffableValue.SetValue(BuffableValue.CurrentValue - _damagePerDelay);
                _damageImpact += _damagePerDelay;
                _delayAccumulator -= _damageDelay;
            }
        }

        protected override bool ConditionCheck()
        {
            return _damageImpact >= _damageCapacity;
        }

        public override void MergeBuffs<TBuff>(TBuff buff)
        {
            var buffTypizied = buff as PoisonBuff;
            if(buffTypizied == null)
            {
                Debug.LogError("Try to cast buff into strange type!");
                return;
            }

            _damageCapacity = buffTypizied._damageCapacity;
            _damageDelay = buffTypizied._damageDelay;
            _damageImpact = buffTypizied._damageImpact;
            BuffableValue = buffTypizied.BuffableValue;
        }
    }
}