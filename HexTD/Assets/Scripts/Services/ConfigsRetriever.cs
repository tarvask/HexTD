using Configs;
using MapEditor;
using Match;
using Match.Field;
using Match.Field.Mob;
using Match.Field.Tower;
using Match.Field.Tower.TowerConfigs;
using Tools;

namespace Services
{
    public class ConfigsRetriever : BaseDisposable, IHexPrefabConfigRetriever, IPropsObjectPrefabConfigRetriever
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

            _levelConfigRetriever = AddDisposable(new MatchesConfigRetriever(_context.FieldConfig.LevelsConfig));
            _hexPrefabConfigRetriever = AddDisposable(new HexPrefabConfigRetriever(_context.FieldConfig.HexagonPrefabConfig));
            _propsObjectPrefabConfigRetriever = AddDisposable(new PropsObjectPrefabConfigRetriever(_context.FieldConfig.PropsPrefabConfig));
            _towerConfigRetriever = AddDisposable(new TowerConfigRetriever(_context.FieldConfig.TowersConfig));
            _mobConfigRetriever = AddDisposable(new MobConfigRetriever(_context.FieldConfig.MobsConfig));
        }

        private readonly Context _context;

        private readonly MatchesConfigRetriever _levelConfigRetriever;
        private readonly HexPrefabConfigRetriever _hexPrefabConfigRetriever;
        private readonly PropsObjectPrefabConfigRetriever _propsObjectPrefabConfigRetriever;
        private readonly TowerConfigRetriever _towerConfigRetriever;
        private readonly MobConfigRetriever _mobConfigRetriever;
        
        public HexObject GetHexByType(string hexTypeName)
        {
            return _hexPrefabConfigRetriever.GetHexByType(hexTypeName);
        }
        
        public PropsPrefabConfig.PropsObjectConfig GetPropsByType(string propsTypeName)
        {
            return _propsObjectPrefabConfigRetriever.GetPropsByType(propsTypeName);
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