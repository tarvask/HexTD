using System;
using System.Collections.Generic;
using BuffLogic;
using BuffLogic.SerializableBuffs;
using Match.Field.Shooting;
using Match.Field.Tower;
using Match.Field.VFX;
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
        [SerializeField] private float projectileSpeed;
        [SerializeField] private ProjectileView projectileView;
        
        [Header("Buffs data")]
        [SerializeReference] private List<BaseSerializableBuff> serializableBuffs;

        public EnumAttackTargetType AttackTargetType => attackTargetType;
        public AttackRangeType AttackRangeType => attackRangeType;
        public float CooldownAndDelay => cooldown + delay;
        public float ProjectileSpeed => projectileSpeed;
        public ProjectileView ProjectileView => projectileView;

        public void ApplyAttackImpact(ITarget targetController, float sqrDistance)
        {
            float damage = targetController.BaseReactiveModel.Damage.Value.Value * baseDamage;
            targetController.Hurt(damage);
            Debug.Log($"Hurt target={targetController.TargetId} by {damage}, current health is {targetController.BaseReactiveModel.Health.Value.CurrentValue}");
        }

        public void ApplyAttackEffect(ITarget attackerController, BuffManager buffManager, VfxManager vfxManager)
        {
            foreach (var serializableBuff in serializableBuffs)
            {
                serializableBuff.ApplyBuff(attackerController, buffManager, vfxManager);
            }
        }
    }
}