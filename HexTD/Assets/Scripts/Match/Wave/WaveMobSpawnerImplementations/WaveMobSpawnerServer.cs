using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace Match.Wave.WaveMobSpawnerImplementations
{
    public class WaveMobSpawnerServer : WaveMobSpawnerBase
    {
        private readonly List<WaveElementChance> _currentPlayer1Reinforcements;
        private readonly List<WaveElementChance> _currentPlayer2Reinforcements;

        public WaveMobSpawnerServer(Context context) : base(context)
        {
            _currentPlayer1Reinforcements = new List<WaveElementChance>(MaxMobsInWave);
            _currentPlayer2Reinforcements = new List<WaveElementChance>(MaxMobsInWave);
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
            
            List<WaveElementDelay> player1NextWaveElementsAndDelays = WaveBuilder.BuildWave(_context.Waves[operatingWaveNumber]);
            List<WaveElementDelay> player2NextWaveElementsAndDelays = new List<WaveElementDelay>(player1NextWaveElementsAndDelays);
            
            List<WaveElementDelay> player1Reinforcement =
                WaveBuilder.BuildReinforcement(_context.Waves[operatingWaveNumber], _currentPlayer1Reinforcements);
            player1NextWaveElementsAndDelays =
                WaveBuilder.AddReinforcementToWave(player1NextWaveElementsAndDelays, player1Reinforcement);
            _currentPlayer1Reinforcements.Clear();

            List<WaveElementDelay> player2Reinforcement =
                WaveBuilder.BuildReinforcement(_context.Waves[operatingWaveNumber], _currentPlayer2Reinforcements);
            player2NextWaveElementsAndDelays =
                WaveBuilder.AddReinforcementToWave(player2NextWaveElementsAndDelays, player2Reinforcement);
            _currentPlayer2Reinforcements.Clear();

            BuiltWaveParams nextBuiltWaveParams = new BuiltWaveParams(
                player1NextWaveElementsAndDelays, player2NextWaveElementsAndDelays,
                _context.Waves[operatingWaveNumber].Duration,
                _context.Waves[operatingWaveNumber].AreArtifactsAvailable,
                _context.Waves[operatingWaveNumber].PauseBeforeWave);
            
            _context.ServerCommands.StartWaveSpawn.Fire(nextBuiltWaveParams, newRandomSeed);
            StartWave(nextBuiltWaveParams, newRandomSeed);
        }
    }
}