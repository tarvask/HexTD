using System;
using BuffLogic;
using Match.Field.Mob;
using Match.Field.Shooting;
using UnityEngine;

namespace Match.Field.Tower.TowerConfigs
{
    [Serializable]
    public abstract class BaseAttackEffect<TTarget>
    {
        [SerializeField] private float projectileSpeed;
        [SerializeField] private ProjectileView projectileView;


        public float ProjectileSpeed => projectileSpeed;
        public ProjectileView ProjectileView => projectileView;

        public abstract void ApplyAttack(TTarget mobController, BuffManager buffManager);
    }
}