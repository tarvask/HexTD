using System.Collections.Generic;
using BuffLogic;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.Tower.TowerConfigs.TowerAttackEffectConfigs
{
    public class SimpleTowerAttack : BaseTowerAttack
    {
        [SerializeField] private float damage;

        public override void ApplyAttackImpact(IShootable mobController, IEnumerable<IBuff<float>> damageBuffs)
        {
            float buffedDamage = BaseBuffableValue<float>.ApplyBuffs(damage, damageBuffs);
            mobController.Hurt(buffedDamage);
        }

        public override void ApplyAttackEffect(IShootable mobController, BuffManager buffManager)
        {
        }
    }
}