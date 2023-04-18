using System;
using BuffLogic;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.AttackEffect
{
    [Serializable]
    public class HealTowerAttack : BaseAttackEffect
    {
        [SerializeField] private float healCapacity;
        [SerializeField] private float healPerDelay;
        [SerializeField] private float delay;

        protected override void ApplyAttackImpact(ITargetable mobController, BuffManager buffManager)
        {
            
        }

        protected override void ApplyAttackEffect(ITargetable targetController, BuffManager buffManager)
        {
            HealBuff healBuff = new HealBuff(healCapacity, healPerDelay, delay);
            buffManager.AddBuff(targetController, healBuff);
        }
    }
}