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
        
        public override void ApplyAttackImpact(ITargetable targetController, float sqrDistance)
        {
            float withDistanceDamage = ApplySplashDamageDistribution(baseDamage, sqrDistance);
            float damage = targetController.BaseReactiveModel.Damage.CopyValue(withDistanceDamage);
            targetController.Hurt(damage);
        }

        public override void ApplyAttackEffect(ITargetable targetController, BuffManager buffManager)
        {
            
        }
    }
}