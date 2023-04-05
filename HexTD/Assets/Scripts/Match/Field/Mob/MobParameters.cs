using System;
using System.ComponentModel;
using UnityEngine;

namespace Match.Field.Mob
{
    [Serializable]
    public class MobParameters
    {
        [SerializeField] private byte typeId;
        [SerializeField] private PowerType powerType;
        [SerializeField] private int healthPoints;
        [SerializeField] private float speed;
        [SerializeField] private int attackPower;
        [SerializeField] private float reloadTime;
        [SerializeField] private bool hasRangeDamage;
        [SerializeField] private float attackRangeRadius;

        [SerializeField] [Description("Reward for killed mob")]
        private int rewardInSilver;
        [SerializeField] [Description("Price of mob in reinforcement")]
        private int priceInSilver;
        [SerializeField] [Description("Earnings per wave appearance")]
        private int incomeInSilver;
        [SerializeField] [Description("Chance in percents of dropping crystal for killed mob")]
        private int crystalDropChance;

        public byte TypeId => typeId;
        public PowerType PowerType => powerType;
        public int HealthPoints => healthPoints;
        public float Speed => speed;
        public int AttackPower => attackPower;
        public float ReloadTime => reloadTime;
        public bool HasRangeDamage => hasRangeDamage;
        public float AttackRangeRadius => attackRangeRadius;

        public int RewardInSilver => rewardInSilver;
        public int PriceInSilver => priceInSilver;
        public int IncomeInSilver => incomeInSilver;
        public int CrystalDropChance => crystalDropChance;
    }
}