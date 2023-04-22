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

        public override void ApplyAttackImpact(ITarget attackerController, float sqrDistance)
        {
            
        }

        public override void ApplyAttackEffect(ITarget attackerController, BuffManager buffManager)
        {
            HealBuff healBuff = new HealBuff(healCapacity, healPerDelay, delay);
            buffManager.AddBuff(attackerController, healBuff);
        }
    }
}