using System;
using BuffLogic;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.AttackEffect
{
    [Serializable]
    public class SimpleTowerAttack : BaseAttackEffect
    {
        [SerializeField] private float baseDamage;

        protected override void ApplyAttackImpact(ITargetable targetController, BuffManager buffManager)
        {
            float damage = targetController.BaseReactiveModel.Damage.CopyValue(baseDamage);
            targetController.Hurt(damage);
        }

        protected override void ApplyAttackEffect(ITargetable targetController, BuffManager buffManager)
        {
            
        }
    }
}