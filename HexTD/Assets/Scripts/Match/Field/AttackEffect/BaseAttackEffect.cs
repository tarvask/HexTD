using System;
using System.Collections.Generic;
using BuffLogic;
using BuffLogic.SerializableBuffs;
using Match.Field.Shooting;
using Match.Field.Tower;
using UnityEngine;

namespace Match.Field.AttackEffect
{
    [Serializable]
    public class BaseAttackEffect
    {
        [Header("Base attack data")]
        [SerializeField] private EnumAttackTargetType attackTargetType;
        [SerializeField] private AttackRangeType attackRangeType;
        [SerializeField] private float baseDamage;
        [SerializeField] private float delay;
        [SerializeField] private float cooldown;
        [SerializeField] private int attackRadiusInHex;
        [SerializeField] private float projectileSpeed;
        [SerializeField] private ProjectileView projectileView;
        [SerializeReference] private List<BaseSerializableBuff> serializableBuffs;

        public EnumAttackTargetType AttackTargetType => attackTargetType;
        public AttackRangeType AttackRangeType => attackRangeType;
        public float CooldownAndDelay => cooldown + delay;
        public int AttackRadiusInHex => attackRadiusInHex;
        public float ProjectileSpeed => projectileSpeed;
        public ProjectileView ProjectileView => projectileView;

        public void ApplyAttackImpact(ITarget attackerController, float sqrDistance)
        {
            float damage = attackerController.BaseReactiveModel.Damage.CopyValue(baseDamage);
            attackerController.Hurt(damage);
        }

        public void ApplyAttackEffect(ITarget attackerController, BuffManager buffManager)
        {
            foreach (var serializableBuff in serializableBuffs)
            {
                serializableBuff.ApplyBuff(attackerController, buffManager);
            }
        }
    }
}