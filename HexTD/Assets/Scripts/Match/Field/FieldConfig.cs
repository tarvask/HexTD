using System.ComponentModel;
using Configs;
using Match.Field.Mob;
using Match.Field.Tower;
using UnityEngine;

namespace Match.Field
{
    [CreateAssetMenu(menuName = "Configs/Match/Field Config")]
    public class FieldConfig : ScriptableObject
    {
        [SerializeField] private int castleHealth;
        [SerializeField] private float energyRestoreDelay;
        [SerializeField] private int energyRestoreValue;
        [SerializeField] private int maxEnergy;
        [SerializeField] private byte maxOverlappingWaves;
        
        [Header("Delays and durations")]
        [SerializeField] [Description("In milliseconds")] private int towerRemovingDuration;
        [SerializeField] [Description("In seconds")] private float matchInfoShowDuration;
        [SerializeField] [Description("In seconds")] private float waveInfoShowDuration;
        [SerializeField] [Description("In seconds")] private float targetArtifactChoosingDuration;
        [SerializeField] [Description("In seconds")] private float technicalPauseBetweenWavesDuration;

        [Header("Hexes")] 
        [SerializeField] private HexSettingsConfig hexSettingsConfig;
        [SerializeField] private HexagonPrefabConfig hexagonPrefabConfig;

        [Header("Towers")]
        [SerializeField] private TowersConfig towersConfig;

        [Header("Mobs")]
        [SerializeField] private MobsConfig mobsConfig;

        public int CastleHealth => castleHealth;
        public float EnergyRestoreDelay => energyRestoreDelay;
        public int EnergyRestoreValue => energyRestoreValue;
        public int MaxEnergy => maxEnergy;
        public byte MaxOverlappingWaves => maxOverlappingWaves;

        // delays and duration
        public int TowerRemovingDuration => towerRemovingDuration;
        public float MatchInfoShowDuration => matchInfoShowDuration;
        public float WaveInfoShowDuration => waveInfoShowDuration;
        public float TargetArtifactChoosingDuration => targetArtifactChoosingDuration;
        public float TechnicalPauseBetweenWavesDuration => technicalPauseBetweenWavesDuration;

        // cells
        public HexSettingsConfig HexSettingsConfig => hexSettingsConfig;
        public HexagonPrefabConfig HexagonPrefabConfig => hexagonPrefabConfig;

        // towers
        public TowersConfig TowersConfig => towersConfig;
        
        // mobs
        public MobsConfig MobsConfig => mobsConfig;
    }
}
