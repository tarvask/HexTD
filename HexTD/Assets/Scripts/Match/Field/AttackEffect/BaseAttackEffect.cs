using System;
using BuffLogic;
using Match.Field.Shooting;
using Match.Field.Tower;
using UnityEngine;

namespace Match.Field.AttackEffect
{
    [Serializable]
    public abstract class BaseAttackEffect
    {
        [Header("Base attack data")]
        [SerializeField] private EnumAttackTargetType attackTargetType;
        [SerializeField] private AttackRangeType attackRangeType;
        [SerializeField] private float delay;
        [SerializeField] private float cooldown;
        [SerializeField] private int attackRadiusInHex;
        [SerializeField] private float projectileSpeed;
        [SerializeField] private ProjectileView projectileView;
        
        public EnumAttackTargetType AttackTargetType => attackTargetType;
        public AttackRangeType AttackRangeType => attackRangeType;
        public float CooldownAndDelay => cooldown + delay;
        public int AttackRadiusInHex => attackRadiusInHex;
        public float ProjectileSpeed => projectileSpeed;
        public ProjectileView ProjectileView => projectileView;

        public abstract void ApplyAttackImpact(ITarget attackerController, float sqrDistance);
        public abstract void ApplyAttackEffect(ITarget attackerController, BuffManager buffManager);
    }
}