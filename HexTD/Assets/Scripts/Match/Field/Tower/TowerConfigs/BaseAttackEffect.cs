using System;
using System.Collections.Generic;
using BuffLogic;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.Tower.TowerConfigs
{
    [Serializable]
    public abstract class BaseAttackEffect<TTarget>
    {
        [SerializeField] private float cooldown;
        [SerializeField] private int attackRadiusInHex;
        [SerializeField] private float projectileSpeed;
        [SerializeField] private ProjectileView projectileView;

        public float Cooldown => cooldown;
        public int AttackRadiusInHex => attackRadiusInHex;
        public float ProjectileSpeed => projectileSpeed;
        public ProjectileView ProjectileView => projectileView;

        public abstract void ApplyAttackImpact(TTarget mobController, IEnumerable<IBuff<float>> damageBuffs);
        public abstract void ApplyAttackEffect(TTarget mobController, BuffManager buffManager);
    }
}