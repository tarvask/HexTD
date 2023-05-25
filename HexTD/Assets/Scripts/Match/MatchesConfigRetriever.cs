using System;
using System.Collections.Generic;
using Tools;

namespace Match
{
    public class MatchesConfigRetriever : BaseDisposable
    {
        public struct Context
        {
            public MatchesConfig MatchesConfig { get; }

            public Context(MatchesConfig matchesConfig)
            {
                MatchesConfig = matchesConfig;
            }
        }

        private readonly Context _context;
        private readonly Dictionary<byte, MatchConfig> _levelsByIds;
        
        public MatchesConfigRetriever(Context context)
        {
            _context = context;
            _levelsByIds = new Dictionary<byte, MatchConfig>(_context.MatchesConfig.Levels.Count);

            foreach (var levelConfigPair in _context.MatchesConfig.Levels)
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