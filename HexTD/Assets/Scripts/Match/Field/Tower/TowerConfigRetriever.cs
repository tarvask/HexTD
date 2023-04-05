using System;
using System.Collections.Generic;
using Tools;

namespace Match.Field.Tower
{
    public class TowerConfigRetriever : BaseDisposable
    {
        public struct Context
        {
            public TowersConfig TowersConfig { get; }

            public Context(TowersConfig towersConfig)
            {
                TowersConfig = towersConfig;
            }
        }

        private readonly Context _context;
        private readonly Dictionary<int, TowerConfig> _towersByIds;
        
        public TowerConfigRetriever(Context context)
        {
            _context = context;
            _towersByIds = new Dictionary<int, TowerConfig>(_context.TowersConfig.Towers.Count);

            foreach (TowerConfig towerConfig in _context.TowersConfig.Towers)
            {
                _towersByIds.Add((int)towerConfig.Parameters.RegularParameters.Data.TowerType, towerConfig);
            }
        }
        
        public TowerConfig GetTowerByType(TowerType towerType)
        {
            if (_towersByIds.TryGetValue((int) towerType, out TowerConfig towerConfig))
                return towerConfig;
            
            throw new ArgumentException("Unknown or undefined tower type");
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _towersByIds.Clear();
        }
    }
}