using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace Match.Wave
{
    public class WaveMobSpawnerServerWithReinforcements : WaveMobSpawnerBaseWithReinforcements
    {
        private readonly List<WaveElementChance> _currentPlayer1Reinforcements;
        private readonly List<WaveElementChance> _currentPlayer2Reinforcements;

        public WaveMobSpawnerServerWithReinforcements(Context context) : base(context)
        {
            _currentPlayer1Reinforcements = new List<WaveElementChance>(WaveMobSpawnerCoordinator.MaxMobsInWave);
            _currentPlayer2Reinforcements = new List<WaveElementChance>(WaveMobSpawnerCoordinator.MaxMobsInWave);
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
            
            List<WaveElementDelayAndPath> player1NextWaveElementsAndDelays = WaveBuilderInRandomOrder.BuildWave(_context.Waves[operatingWaveNumber]);
            List<WaveElementDelayAndPath> player2NextWaveElementsAndDelays = new List<WaveElementDelayAndPath>(player1NextWaveElementsAndDelays);
            
            List<WaveElementDelayAndPath> player1Reinforcement =
                WaveBuilderInRandomOrder.BuildReinforcement(_context.Waves[operatingWaveNumber], _currentPlayer1Reinforcements);
            player1NextWaveElementsAndDelays =
                WaveBuilderInRandomOrder.AddReinforcementToWave(player1NextWaveElementsAndDelays, player1Reinforcement);
            _currentPlayer1Reinforcements.Clear();

            List<WaveElementDelayAndPath> player2Reinforcement =
                WaveBuilderInRandomOrder.BuildReinforcement(_context.Waves[operatingWaveNumber], _currentPlayer2Reinforcements);
            player2NextWaveElementsAndDelays =
                WaveBuilderInRandomOrder.AddReinforcementToWave(player2NextWaveElementsAndDelays, player2Reinforcement);
            _currentPlayer2Reinforcements.Clear();

            BuiltWaveParams nextBuiltWaveParams = new BuiltWaveParams(
                player1NextWaveElementsAndDelays, player2NextWaveElementsAndDelays,
                _context.Waves[operatingWaveNumber].Duration,
                _context.Waves[operatingWaveNumber].PauseBeforeWave);
            
            _context.ServerCommands.StartWaveSpawn.Fire(nextBuiltWaveParams, newRandomSeed);
            StartWave(nextBuiltWaveParams, newRandomSeed);
        }
    }
}