using System;
using System.Collections.Generic;
using Tools;

namespace Match.Field.Mob
{
    public class MobConfigRetriever : BaseDisposable
    {
        public struct Context
        {
            public MobsConfig MobsConfig { get; }

            public Context(MobsConfig mobsConfig)
            {
                MobsConfig = mobsConfig;
            }
        }

        private readonly Context _context;
        private readonly List<MobConfig> _mobsList;
        private readonly Dictionary<byte, MobConfig> _mobsConfigsDictionary;
        
        public MobConfigRetriever(Context context)
        {
            _context = context;

            _mobsList = new List<MobConfig>(_context.MobsConfig.WaterMobs.Length
                                            + _context.MobsConfig.FireMobs.Length
                                            + _context.MobsConfig.EarthMobs.Length
                                            + _context.MobsConfig.NatureMobs.Length
                                            + _context.MobsConfig.DeathMobs.Length
                                            + _context.MobsConfig.Troops.Length);
            _mobsList.AddRange(_context.MobsConfig.WaterMobs);
            _mobsList.AddRange(_context.MobsConfig.FireMobs);
            _mobsList.AddRange(_context.MobsConfig.EarthMobs);
            _mobsList.AddRange(_context.MobsConfig.NatureMobs);
            _mobsList.AddRange(_context.MobsConfig.DeathMobs);
            _mobsList.AddRange(_context.MobsConfig.Troops);

            _mobsConfigsDictionary = FillMobsDictionary();
        }

        private Dictionary<byte, MobConfig> FillMobsDictionary()
        {
            Dictionary<byte, MobConfig> mobsDictionary = new Dictionary<byte, MobConfig>(_mobsList.Count);

            foreach (MobConfig mobConfig in _mobsList)
            {
                mobsDictionary.Add(mobConfig.Parameters.TypeId, mobConfig);
                //mobsDictionary.Add(RaceAndPowerToKey(mobConfig.Parameters.RaceType, mobConfig.Parameters.PowerType), mobConfig);
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