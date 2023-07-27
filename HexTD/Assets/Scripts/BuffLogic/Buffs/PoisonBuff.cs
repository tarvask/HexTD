using ExitGames.Client.Photon;
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

        protected override void UpdateBuff(float frameLength)
        {
            _delayAccumulator += frameLength;

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
        
        public override Hashtable ToNetwork()
        {
            return new Hashtable()
            {
                {Match.Serialization.SerializerToNetwork.SerializedType, typeof(PoisonBuff)},
                {$"{Match.Serialization.SerializerToNetwork.SerializedType}In", typeof(FloatImpactableBuffableValue)},
                { nameof(_damageCapacity), _damageCapacity },
                { nameof(_damagePerDelay), _damagePerDelay },
                { nameof(_damageDelay), _damageDelay },
                { nameof(_delayAccumulator), _delayAccumulator },
                { nameof(_damageImpact), _damageImpact }
            };
        }
        
        public override object Restore(Hashtable hashtable)
        {
            float damageCapacity = (float)hashtable[nameof(_damageCapacity)];
            float damagePerDelay = (float)hashtable[nameof(_damagePerDelay)];
            float damageDelay = (float)hashtable[nameof(_damageDelay)];
            float delayAccumulator = (float)hashtable[nameof(_delayAccumulator)];
            float damageImpact = (float)hashtable[nameof(_damageImpact)];

            var healBuff = new PoisonBuff(damageCapacity, damagePerDelay, damageDelay);
            healBuff._delayAccumulator = delayAccumulator;
            healBuff._damageImpact = damageImpact;

            return healBuff;
        }
    }
}