using System;
using BuffLogic;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.AttackEffect
{
    [Serializable]
    public abstract class BaseAttackEffect
    {
        [Header("Base attack data")]
        [SerializeField] private EnumAttackTargetType attackTargetType;
        [SerializeField] private float cooldown;
        [SerializeField] private int attackRadiusInHex;
        [SerializeField] private float projectileSpeed;
        [SerializeField] private ProjectileView projectileView;
        
        public EnumAttackTargetType AttackTargetType => attackTargetType;
        public float Cooldown => cooldown;
        public int AttackRadiusInHex => attackRadiusInHex;
        public float ProjectileSpeed => projectileSpeed;
        public ProjectileView ProjectileView => projectileView;

        public abstract void ApplyAttackImpact(ITargetable targetController, float sqrDistance);
        public abstract void ApplyAttackEffect(ITargetable targetController, BuffManager buffManager);
    }
}