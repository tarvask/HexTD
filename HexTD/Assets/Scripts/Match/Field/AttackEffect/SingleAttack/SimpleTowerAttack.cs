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

        public void ApplyAttackImpact(ITarget attackerController, float sqrDistance)
        {
            float damage = attackerController.BaseReactiveModel.Damage.CopyValue(baseDamage);
            attackerController.Hurt(damage);
        }

        public void ApplyAttackEffect(ITarget attackerController, BuffManager buffManager)
        {
            
        }
    }
}