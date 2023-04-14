using System;
using BuffLogic;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.AttackEffect
{
    [Serializable]
    public abstract class BaseAttackEffect
    {
        [SerializeField] private EnumAttackTarget attackTarget;
        [SerializeField] private float cooldown;
        [SerializeField] private int attackRadiusInHex;
        [SerializeField] private float projectileSpeed;
        [SerializeField] private ProjectileView projectileView;

        public EnumAttackTarget AttackTarget => attackTarget;
        public float Cooldown => cooldown;
        public int AttackRadiusInHex => attackRadiusInHex;
        public float ProjectileSpeed => projectileSpeed;
        public ProjectileView ProjectileView => projectileView;

        public void ApplyAttack(ITargetable targetController, BuffManager buffManager)
        {
            ApplyAttackImpact(targetController, buffManager);
            ApplyAttackEffect(targetController, buffManager);
        }
        
        protected abstract void ApplyAttackImpact(ITargetable targetController, BuffManager buffManager);
        protected abstract void ApplyAttackEffect(ITargetable targetController, BuffManager buffManager);
    }
}