using MapEditor;
using Match;
using Match.Field;
using Match.Field.Mob;
using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
using Tools;

namespace Services
{
    public class ConfigsRetriever : BaseDisposable
    {
        public struct Context
        {
            public FieldConfig FieldConfig { get; }

            public Context(FieldConfig fieldConfig)
            {
                FieldConfig = fieldConfig;
            }
        }

        public int TowerCount => _context.FieldConfig.TowersConfig.Towers.Count;

        public ConfigsRetriever(Context context)
        {
            _context = context;

            MatchesConfigRetriever.Context levelConfigRetrieverContext =
                new MatchesConfigRetriever.Context(_context.FieldConfig.LevelsConfig);
            _levelConfigRetriever = AddDisposable(new MatchesConfigRetriever(levelConfigRetrieverContext));
            FieldConfigCellsRetriever.Context fieldConfigCellsRetrieverContext =
                new FieldConfigCellsRetriever.Context(_context.FieldConfig);
            _fieldConfigCellsRetriever = AddDisposable(new FieldConfigCellsRetriever(fieldConfigCellsRetrieverContext));
            TowerConfigRetriever.Context towerConfigRetrieverContext =
                new TowerConfigRetriever.Context(_context.FieldConfig.TowersConfig);
            _towerConfigRetriever = AddDisposable(new TowerConfigRetriever(towerConfigRetrieverContext));
            MobConfigRetriever.Context mobConfigRetrieverContext =
                new MobConfigRetriever.Context(_context.FieldConfig.MobsConfig);
            _mobConfigRetriever = AddDisposable(new MobConfigRetriever(mobConfigRetrieverContext));
        }

        private readonly Context _context;

        private readonly MatchesConfigRetriever _levelConfigRetriever;
        private readonly FieldConfigCellsRetriever _fieldConfigCellsRetriever;
        private readonly TowerConfigRetriever _towerConfigRetriever;
        private readonly MobConfigRetriever _mobConfigRetriever;
        
        public HexObject GetCellByType(string hexTypeName)
        {
            return _fieldConfigCellsRetriever.GetCellByType(hexTypeName);
        }

        public TowerConfigNew GetTowerByType(TowerType towerType)
        {
            return _towerConfigRetriever.GetTowerByType(towerType);
        }
        
        public MobConfig GetMobById(byte mobId)
        {
            return _mobConfigRetriever.GetMobById(mobId);
        }

        public MatchConfig GetLevelById(byte levelId)
        {
            return _levelConfigRetriever.GetLevelById(levelId);
        }
    }
}