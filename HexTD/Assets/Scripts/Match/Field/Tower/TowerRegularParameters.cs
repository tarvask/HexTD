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
        [SerializeField] private TowerPlacementType placementType;
        [SerializeField] private ReachableAttackTargetFinderType reachableAttackTargetFinderType;
        [SerializeField] private TargetFindingTacticType targetFindingTacticType;
        
        [Header("Out of using")]
        [SerializeField] private RaceType raceType;
        [SerializeField] private byte epicDegree;
        
        public TowerType TowerType => towerType;
        public string TowerName => towerName;
        public RaceType RaceType => raceType;
        public byte EpicDegree => epicDegree;
        public TowerPlacementType PlacementType => placementType;
        public ReachableAttackTargetFinderType ReachableAttackTargetFinderType => reachableAttackTargetFinderType;
        public TargetFindingTacticType TargetFindingTacticType => targetFindingTacticType;
        public bool PreferUnbuffedTargets => preferUnbuffedTargets;
        public bool ResetTargetEveryShot => resetTargetEveryShot;

        public static class FieldNames
        {
            public static string TowerType => nameof(towerType);
            public static string TowerName => nameof(towerName);
            public static string RaceType => nameof(raceType);
            public static string EpicDegree => nameof(epicDegree);
            public static string ReachableAttackTargetFinderType => nameof(reachableAttackTargetFinderType);
            public static string TargetFindingTacticType => nameof(targetFindingTacticType);
            public static string PreferUnbuffedTargets => nameof(preferUnbuffedTargets);
            public static string ResetTargetEveryShot => nameof(resetTargetEveryShot);
        }
    }
}