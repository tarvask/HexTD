using System;
using BuffLogic;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.AttackEffect
{
    [Serializable]
    public class SplashSimpleAttack : BaseSplashAttack
    {
        [SerializeField] private float baseDamage;
        
        public override void ApplyAttackImpact(ITarget attackerController, float sqrDistance)
        {
            float withDistanceDamage = ApplySplashDamageDistribution(baseDamage, sqrDistance);
            float damage = attackerController.BaseReactiveModel.Damage.CopyValue(withDistanceDamage);
            attackerController.Hurt(damage);
        }

        public override void ApplyAttackEffect(ITarget attackerController, BuffManager buffManager)
        {
            
        }
    }
}