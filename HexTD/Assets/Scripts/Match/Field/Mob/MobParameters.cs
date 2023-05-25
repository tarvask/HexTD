using System;
using System.ComponentModel;
using UnityEngine;

namespace Match.Field.Mob
{
    [Serializable]
    public class MobParameters
    {
        [SerializeField] private byte typeId;
        [SerializeField] private int healthPoints;
        [SerializeField] private float speed;
        [SerializeField] private int attackPower;
        [SerializeField] private float delay;
        [SerializeField] private float cooldown;
        //[SerializeField] private bool hasRangeDamage;
        //[SerializeField] private float attackRangeRadius;
        [SerializeField] private bool isBoss;

        [SerializeField] [Description("Reward for killed mob")]
        private int rewardInCoins;

        public byte TypeId => typeId;
        public int HealthPoints => healthPoints;
        public float Speed => speed;
        public int AttackPower => attackPower;
        public float Delay => delay;
        public float Cooldown => cooldown;
        //public bool HasRangeDamage => hasRangeDamage;
        //public float AttackRangeRadius => attackRangeRadius;
        public bool IsBoss => isBoss;

        public int RewardInCoins => rewardInCoins;
    }
}