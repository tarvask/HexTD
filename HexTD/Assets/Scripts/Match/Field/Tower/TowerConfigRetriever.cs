using System;
using System.Collections.Generic;
using Match.Field.Tower.TowerConfigs;
using Tools;

namespace Match.Field.Tower
{
    public class TowerConfigRetriever : BaseDisposable
    {
        public struct Context
        {
            public TowerConfigsNew TowersConfig { get; }

            public Context(TowerConfigsNew towersConfig)
            {
                TowersConfig = towersConfig;
            }
        }

        private readonly Context _context;
        private readonly Dictionary<byte, TowerConfigNew> _towersByIds;
        
        public TowerConfigRetriever(Context context)
        {
            _context = context;
            _towersByIds = new Dictionary<byte, TowerConfigNew>(_context.TowersConfig.Towers.Count);

            foreach (TowerConfigNew towerConfig in _context.TowersConfig.Towers)
            {
                _towersByIds.Add((byte)towerConfig.RegularParameters.TowerType, towerConfig);
            }
        }
        
        public TowerConfigNew GetTowerByType(TowerType towerType)
        {
            if (_towersByIds.TryGetValue((byte)towerType, out TowerConfigNew towerConfig))
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