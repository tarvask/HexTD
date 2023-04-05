using System.ComponentModel;
using Match.Field.Mob;
using Match.Field.Tower;
using UnityEngine;

namespace Match.Field
{
    [CreateAssetMenu(menuName = "Configs/Match/Field Config")]
    public class FieldConfig : ScriptableObject
    {
        [SerializeField] private int fieldWidthInCells;
        [SerializeField] private int fieldHeightInCells;

        [Space]
        [SerializeField] private int castleHealth;
        [SerializeField] private int reinforcementsSize;
        [SerializeField] private byte maxOverlappingWaves;
        
        [Header("Delays and durations")]
        [SerializeField] [Description("In milliseconds")] private int towerRemovingDuration;
        [SerializeField] [Description("In seconds")] private float matchInfoShowDuration;
        [SerializeField] [Description("In seconds")] private float waveInfoShowDuration;
        [SerializeField] [Description("In seconds")] private float targetArtifactChoosingDuration;
        [SerializeField] [Description("In seconds")] private float technicalPauseBetweenWavesDuration;

        [Header("Cells")]
        [SerializeField] private GameObject groundPrefab;
        
        [SerializeField] private GameObject freeCellPrefab;
        [SerializeField] private GameObject roadCellPrefab;
        [SerializeField] private GameObject unavailableCellPrefab;
        [SerializeField] private GameObject blockerCellPrefab;
        [SerializeField] private GameObject towerCellPrefab;
        [SerializeField] private GameObject castleCellPrefab;

        [Header("Towers")]
        [SerializeField] private TowersConfig towersConfig;

        [Header("Mobs")]
        [SerializeField] private MobsConfig mobsConfig;

        public int FieldWidthInCells => fieldWidthInCells;
        public int FieldHeightInCells => fieldHeightInCells;

        public int CastleHealth => castleHealth;
        public int ReinforcementsSize => reinforcementsSize;
        public byte MaxOverlappingWaves => maxOverlappingWaves;

        // delays and duration
        public int TowerRemovingDuration => towerRemovingDuration;
        public float MatchInfoShowDuration => matchInfoShowDuration;
        public float WaveInfoShowDuration => waveInfoShowDuration;
        public float TargetArtifactChoosingDuration => targetArtifactChoosingDuration;
        public float TechnicalPauseBetweenWavesDuration => technicalPauseBetweenWavesDuration;

        // cells
        public GameObject GroundPrefab => groundPrefab;

        public GameObject FreeCellPrefab => freeCellPrefab;
        public GameObject RoadCellPrefab => roadCellPrefab;
        public GameObject UnavailableCellPrefab => unavailableCellPrefab;
        public GameObject BlockerCellPrefab => blockerCellPrefab;
        public GameObject TowerCellPrefab => towerCellPrefab;
        public GameObject CastleCellPrefab => castleCellPrefab;

        // towers
        public TowersConfig TowersConfig => towersConfig;
        
        // mobs
        public MobsConfig MobsConfig => mobsConfig;
    }
}
