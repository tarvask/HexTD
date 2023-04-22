using System;
using UnityEngine;

namespace Match.Field.AttackEffect
{
    [Serializable]
    public abstract class BaseSplashAttack : BaseAttackEffect
    {
        [Space]
        [Header("Splash Data")]
        [SerializeField] private SplashShootType splashShootType;
        [SerializeField] private SplashDamageDistributionType splashDamageDistributionType;
        [SerializeField] private int splashRadiusInHex;

        public SplashShootType SplashShootType => splashShootType;
        public int SplashRadiusInHex => splashRadiusInHex;

        protected float ApplySplashDamageDistribution(float impact, float sqrDistance)
        {
            switch (splashDamageDistributionType)
            {
                case SplashDamageDistributionType.Constant:
                    return impact;
            }

            Debug.LogError("SplashDamageDistributionType is Undefined or without define!");
            return impact;
        }
    }
}