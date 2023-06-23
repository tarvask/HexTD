using System;
using UnityEngine;

namespace Match.Field.AttackEffect
{
    [Serializable]
    public class BaseSingleAttack : BaseAttackEffect
    {
        [Space]
        [Header("Single attack data")]
        [SerializeField] private int attackRadiusInHex;
        [SerializeField] private float splashRadiusInUnits;
        
        public int AttackRadiusInHex => attackRadiusInHex;
        public float SplashRadiusInUnits => splashRadiusInUnits;
    }
}