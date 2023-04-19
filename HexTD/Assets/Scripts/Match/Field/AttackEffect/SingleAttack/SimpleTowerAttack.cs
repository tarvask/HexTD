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

        public override void ApplyAttackImpact(ITargetable targetController, float sqrDistance)
        {
            float damage = targetController.BaseReactiveModel.Damage.CopyValue(baseDamage);
            targetController.Hurt(damage);
        }

        public override void ApplyAttackEffect(ITargetable targetController, BuffManager buffManager)
        {
            
        }
    }
}