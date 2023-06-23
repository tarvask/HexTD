using System;
using System.Collections.Generic;
using Match.Field.Tower.TowerConfigs;
using Tools;

namespace Match.Field.Tower
{
    public class TowerConfigRetriever : BaseDisposable
    {
        private readonly Dictionary<byte, TowerConfigNew> _towersByIds;
        
        public TowerConfigRetriever(TowerConfigsNew towersConfig)
        {
            _towersByIds = new Dictionary<byte, TowerConfigNew>(towersConfig.Towers.Count);

            foreach (TowerConfigNew towerConfig in towersConfig.Towers)
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