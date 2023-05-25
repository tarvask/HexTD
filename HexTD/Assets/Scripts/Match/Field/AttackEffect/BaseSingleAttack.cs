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
        
        public int AttackRadiusInHex => attackRadiusInHex;
    }
}