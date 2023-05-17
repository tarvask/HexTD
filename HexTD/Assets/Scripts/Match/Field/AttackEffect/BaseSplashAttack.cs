using System;
using UnityEngine;

namespace Match.Field.AttackEffect
{
    [Serializable]
    public class BaseSplashAttack : BaseAttackEffect
    {
        [Space]
        [Header("Splash Data")]
        [SerializeField] private int splashRadiusInHex;

        public int SplashRadiusInHex => splashRadiusInHex;

        protected float ApplySplashDamageDistribution(float impact, float sqrDistance)
        {
            return impact;
        }
    }
}