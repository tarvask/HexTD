using System;
using UnityEngine;

namespace Match.Field.Tower
{
    [Serializable]
    public class TowerLevelRegularParams
    {
        [SerializeField] private int level;
        [SerializeField] private float attackPower;
        [SerializeField] private float reloadTime;
        [SerializeField] private int attackRadiusInHexCount;
        [SerializeField] private float projectileSpeed;
        [SerializeField] private int energyCost;
        [SerializeField] private int energyRefund;
        [SerializeField] private float buildingTime;
        
        public int Level => level;
        public float AttackPower => attackPower;
        public float ReloadTime => reloadTime;
        public int AttackRadiusInHexCount => attackRadiusInHexCount;
        public float ProjectileSpeed => projectileSpeed;
        public int EnergyCost => energyCost;
        public int EnergyRefund => energyRefund;
        public float BuildingTime => buildingTime;

        public static class FieldNames
        {
            public static string Level => nameof(level);
            public static string AttackPower => nameof(attackPower);
            public static string ReloadTime => nameof(reloadTime);
            public static string AttackRadius => nameof(attackRadiusInHexCount);
            public static string ProjectileSpeed => nameof(projectileSpeed);
            public static string Price => nameof(energyCost);
            public static string RefundPrice => nameof(energyRefund);
            public static string BuildingTime => nameof(buildingTime);
        }
    }
}