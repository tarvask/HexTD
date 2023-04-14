using System;
using System.Collections.Generic;
using BuffLogic;
using Match.Field.Mob;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.Tower.TowerConfigs.TowerAttackEffectConfigs
{
    [Serializable]
    public class HealTowerAttack : BaseTowerAttack
    {
        [SerializeField] private float healCapacity;
        [SerializeField] private float healPerDelay;
        [SerializeField] private float delay;

        public override void ApplyAttackImpact(IShootable mobController, IEnumerable<IBuff<float>> damageBuffs)
        {
            throw new NotImplementedException();
        }

        public override void ApplyAttackEffect(IShootable mobController, BuffManager buffManager)
        {
            HealBuff poisonBuff = new HealBuff(healCapacity, healPerDelay, delay);
            buffManager.AddBuff(mobController, poisonBuff);
        }
    }
}