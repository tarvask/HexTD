using ExitGames.Client.Photon;
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

        protected override void UpdateBuff(float frameLength)
        {
            _delayAccumulator += frameLength;

            while (_delayAccumulator >= _healDelay)
            {
                BuffableValue.SetValue(BuffableValue.CurrentValue + _healPerDelay);
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
            var typedBuff = buff as HealBuff;
            if (typedBuff == null)
            {
                Debug.LogError("Try to cast buff into strange type!");
                return;
            }

            _healCapacity = typedBuff._healCapacity;
            _healDelay = typedBuff._healDelay;
            _healImpact = typedBuff._healImpact;
            BuffableValue = typedBuff.BuffableValue;
        }
        
        public override Hashtable ToNetwork()
        {
            return new Hashtable()
            {
                {Match.Serialization.SerializerToNetwork.SerializedType, typeof(HealBuff).FullName},
                {nameof(_healCapacity), _healCapacity},
                {nameof(_healPerDelay), _healPerDelay},
                {nameof(_healDelay), _healDelay},
                {nameof(_delayAccumulator), _delayAccumulator},
                {nameof(_healImpact), _healImpact},
            };
        }

        public static object FromNetwork(Hashtable hashtable)
        {
            float healCapacity = (float)hashtable[nameof(_healCapacity)];
            float healPerDelay = (float)hashtable[nameof(_healPerDelay)];
            float healDelay = (float)hashtable[nameof(_healDelay)];
            float delayAccumulator = (float)hashtable[nameof(_delayAccumulator)];
            float healImpact = (float)hashtable[nameof(_healImpact)];

            var healBuff = new HealBuff(healCapacity, healPerDelay, healDelay);
            healBuff._delayAccumulator = delayAccumulator;
            healBuff._healImpact = healImpact;

            return healBuff;
        }
    }
}