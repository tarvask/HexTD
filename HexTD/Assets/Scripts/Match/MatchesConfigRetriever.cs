using System;
using System.Collections.Generic;
using Tools;

namespace Match
{
    public class MatchesConfigRetriever : BaseDisposable
    {
        private readonly Dictionary<byte, MatchConfig> _levelsByIds;
        
        public MatchesConfigRetriever(in MatchesConfig matchesConfig)
        {
            _levelsByIds = new Dictionary<byte, MatchConfig>(matchesConfig.Levels.Count);

            foreach (var levelConfigPair in matchesConfig.Levels)
            {
                _levelsByIds.Add(levelConfigPair.Key, levelConfigPair.Value);
            }
        }
        
        public MatchConfig GetLevelById(byte levelKey)
        {
            if (_levelsByIds.TryGetValue(levelKey, out MatchConfig towerConfig))
                return towerConfig;
            
            throw new ArgumentException("Unknown level");
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _levelsByIds.Clear();
        }
    }
}