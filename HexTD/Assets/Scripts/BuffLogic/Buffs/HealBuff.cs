using UnityEngine;

namespace BuffLogic
{
    public class HealBuff : BaseUnitBuff
    {
        private float _healCapacity;
        private float _healPerDelay;
        private float _healDelay;
        
        private float _delayAccumulator;
        private float _healImpact;
        
        public HealBuff(float healCapacity, float healPerDelay, float healDelay = 4f/60f)
        {
            _healCapacity = healCapacity;
            _healPerDelay = healPerDelay;
            _healDelay = healDelay;
            
            _delayAccumulator = 0f;
            _healImpact = 0f;
        }

        protected override void UpdateBuff()
        {
            _delayAccumulator += Time.deltaTime;

            while (_delayAccumulator >= _healDelay)
            {
                BuffableValue.Hurt(-_healPerDelay);
                _healImpact += _healPerDelay;
                _delayAccumulator -= _healDelay;
            }
        }

        protected override bool ConditionCheck()
        {
            return _healImpact >= _healCapacity;
        }

        public override void MergeBuffs<TBuff>(TBuff buff)
        {
            var buffTypizied = buff as HealBuff;
            if(buffTypizied == null)
            {
                Debug.LogError("Try to cast buff into strange type!");
                return;
            }

            _healCapacity = buffTypizied._healCapacity;
            _healDelay = buffTypizied._healDelay;
            _healImpact = buffTypizied._healImpact;
            BuffableValue = buffTypizied.BuffableValue;
        }
    }
}