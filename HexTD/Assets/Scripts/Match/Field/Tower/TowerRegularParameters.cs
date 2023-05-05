using System;
using Match.Field.Mob;
using Match.Field.Shooting.TargetFinding;
using UnityEngine;

namespace Match.Field.Tower
{
    [Serializable]
    public class TowerRegularParameters
    {
        [SerializeField] private TowerType towerType;
        [SerializeField] private string towerName;
        [SerializeField] private bool preferUnbuffedTargets;
        [SerializeField] private bool resetTargetEveryShot;
        [SerializeField] private TowerPlacementType allowedPositions;
        
        [SerializeField] private TargetFindingTacticType targetFindingTacticType;
        [SerializeField] private byte maxEnemyBlocked;
        
        // out of using
        private RaceType raceType;
        
        public TowerType TowerType => towerType;
        public string TowerName => towerName;
        public RaceType RaceType => raceType;
        public bool PreferUnbuffedTargets => preferUnbuffedTargets;
        public bool ResetTargetEveryShot => resetTargetEveryShot;
        public TowerPlacementType AllowedPositions => allowedPositions;
        public TargetFindingTacticType TargetFindingTacticType => targetFindingTacticType;
        public byte MaxEnemyBlocked => maxEnemyBlocked;

        public static class FieldNames
        {
            public static string TowerType => nameof(towerType);
            public static string TowerName => nameof(towerName);
            public static string RaceType => nameof(raceType);
            public static string TargetFindingTacticType => nameof(targetFindingTacticType);
            public static string PreferUnbuffedTargets => nameof(preferUnbuffedTargets);
            public static string ResetTargetEveryShot => nameof(resetTargetEveryShot);
            public static string MaxEnemyBlocked => nameof(maxEnemyBlocked);
        }
    }
}