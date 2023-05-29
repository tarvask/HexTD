using System;
using System.Collections.Generic;
using Tools;

namespace Match.Field.Mob
{
    public class MobConfigRetriever : BaseDisposable
    {
        private readonly List<MobConfig> _mobsList;
        private readonly Dictionary<byte, MobConfig> _mobsConfigsDictionary;
        
        public MobConfigRetriever(MobsConfig mobsConfig)
        {
            _mobsList = new List<MobConfig>(mobsConfig.Mobs.Length);
            _mobsList.AddRange(mobsConfig.Mobs);

            _mobsConfigsDictionary = FillMobsDictionary();
        }

        private Dictionary<byte, MobConfig> FillMobsDictionary()
        {
            Dictionary<byte, MobConfig> mobsDictionary = new Dictionary<byte, MobConfig>(_mobsList.Count);

            foreach (MobConfig mobConfig in _mobsList)
            {
                mobsDictionary.Add(mobConfig.Parameters.TypeId, mobConfig);
            }

            return mobsDictionary;
        }

        public MobConfig GetMobById(byte mobId)
        {
            if (_mobsConfigsDictionary.TryGetValue(mobId, out MobConfig mobConfig))
                return mobConfig;
            
            throw new ArgumentException($"Unknown or undefined mob id = {mobId}");
        }
    }
}