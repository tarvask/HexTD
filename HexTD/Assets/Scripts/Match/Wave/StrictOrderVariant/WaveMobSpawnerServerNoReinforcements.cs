using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace Match.Wave
{
    public class WaveMobSpawnerServerNoReinforcements : WaveMobSpawnerBaseNoReinforcements
    {
        public WaveMobSpawnerServerNoReinforcements(Context context) : base(context)
        {
        }
        
        protected override void RoleSpecialConstructorActions()
        {
            
        }
        
        protected override void NextWave()
        {
            // set new seed for every wave
            int newRandomSeed = Randomizer.CurrentSeed + 1;
            
            // do not increase for starting wave
            byte nextWaveNumber = (byte)(CurrentWaveNumber + 1);

            // show last wave as many times as needed
            byte operatingWaveNumber = (byte)Mathf.Min(nextWaveNumber, _context.Waves.Length - 1);
            
            List<WaveElementDelayAndPath> player1NextWaveElementsAndDelays = WaveBuilderInStrictOrder.BuildWave(_context.Waves[operatingWaveNumber]);
            List<WaveElementDelayAndPath> player2NextWaveElementsAndDelays = new List<WaveElementDelayAndPath>(player1NextWaveElementsAndDelays);

            BuiltWaveParams nextBuiltWaveParams = new BuiltWaveParams(
                player1NextWaveElementsAndDelays, player2NextWaveElementsAndDelays,
                _context.Waves[operatingWaveNumber].WaveParameters.Duration,
                _context.Waves[operatingWaveNumber].WaveParameters.PauseBeforeWave);
            
            _context.ServerCommands.StartWaveSpawn.Fire(nextBuiltWaveParams, newRandomSeed);
            StartWave(nextBuiltWaveParams, newRandomSeed);
        }
    }
}