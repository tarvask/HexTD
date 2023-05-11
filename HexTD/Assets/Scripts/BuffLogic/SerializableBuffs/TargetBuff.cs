using System;
using Match.Field.Shooting;
using UnityEngine;

namespace BuffLogic.SerializableBuffs
{
    [Serializable]
    public class TargetBuff : BaseSerializableBuff
    {
        private enum InfluenceType
        {
            Undefined = 0,
        
            Heal = 1,
            Poison = 2
        }

        [SerializeField] private InfluenceType influenceType;
        [SerializeField] private float capacity;
        [SerializeField] private float impactPerDelay;
        [SerializeField] private float delay;
        
        public override void ApplyBuff(ITarget target, BuffManager buffManager)
        {
            BaseUnitBuff unitBuff = GetTypedBuff();
            buffManager.AddBuff(target, unitBuff);
        }

        private BaseUnitBuff GetTypedBuff()
        {
            switch (influenceType)
            {
                case InfluenceType.Poison:
                    return new PoisonBuff(capacity, impactPerDelay, delay);
                case InfluenceType.Heal:
                    return new HealBuff(capacity, impactPerDelay, delay);
            }

            throw new Exception("Buff with unknown influence type!");
        }
    }
}