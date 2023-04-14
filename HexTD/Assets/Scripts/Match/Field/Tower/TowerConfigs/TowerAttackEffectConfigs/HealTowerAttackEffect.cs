using System;
using BuffLogic;
using Match.Field.Mob;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.Tower.TowerConfigs.TowerAttackEffectConfigs
{
    [Serializable]
    public class HealTowerAttackEffect : BaseTowerAttackEffect
    {
        [SerializeField] private float healCapacity;
        [SerializeField] private float healPerDelay;
        [SerializeField] private float delay;

        public override void ApplyAttack(IShootable mobController, BuffManager buffManager)
        {
            HealBuff poisonBuff = new HealBuff(healCapacity, healPerDelay, delay);
            buffManager.AddBuff(mobController, poisonBuff);
        }
    }
}