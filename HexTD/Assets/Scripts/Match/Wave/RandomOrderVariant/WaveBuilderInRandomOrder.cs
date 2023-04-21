using System.Collections.Generic;
using Tools;

namespace Match.Wave
{
    public static class WaveBuilderInRandomOrder
    {
        private static List<byte> _waveElements;
        private static HashSet<byte> _randomIndexesOfReinforcements;
        private static List<byte> _randomIndexesShuffledList;

        static WaveBuilderInRandomOrder()
        {
            _waveElements = new List<byte>(WaveMobSpawnerCoordinator.MaxMobsInWave);
            _randomIndexesOfReinforcements = new HashSet<byte>();
            _randomIndexesShuffledList = new List<byte>(WaveMobSpawnerCoordinator.MaxMobsInWave);
        }
        
        public static List<WaveElementDelay> BuildWave(WaveParametersWithChances waveParametersWithChances)
        {
            PrepareWaveElements(waveParametersWithChances);
            List<WaveElementDelay> mobsWithDelays = new List<WaveElementDelay>(_waveElements.Count);

            foreach (byte waveElement in _waveElements)
            {
                mobsWithDelays.Add(new WaveElementDelay(waveElement, GetRandomWavePause(waveParametersWithChances)));
            }

            return mobsWithDelays;
        }

        public static List<WaveElementDelay> BuildReinforcement(WaveParametersWithChances waveParametersWithChances,
            List<WaveElementChance> reinforcementElements)
        {
            List<WaveElementDelay> mobsWithDelays = new List<WaveElementDelay>(reinforcementElements.Count);

            foreach (WaveElementChance waveElement in reinforcementElements)
            {
                for (int i = 0; i < waveElement.MaxCount; i++)
                {
                    mobsWithDelays.Add(new WaveElementDelay(waveElement.MobId, GetRandomReinforcementPause(waveParametersWithChances)));
                }
            }

            return mobsWithDelays;
        }

        public static List<WaveElementDelay> AddReinforcementToWave(List<WaveElementDelay> waveElements,
            List<WaveElementDelay> reinforcementElements)
        {
            byte waveWithReinforcementsLength = (byte)(waveElements.Count + reinforcementElements.Count);
            List<WaveElementDelay> mobsWithDelays = new List<WaveElementDelay>(waveWithReinforcementsLength);
            PrepareRandomIndexes(waveElements, reinforcementElements);

            // insert reinforcements and wave elements to their places
            for (byte mobIndex = 0, waveElementIndex = 0, reinforcementIndex = 0; mobIndex < waveWithReinforcementsLength; mobIndex++)
            {
                if (_randomIndexesOfReinforcements.Contains(mobIndex))
                {
                    mobsWithDelays.Add(reinforcementElements[reinforcementIndex]);
                    reinforcementIndex++;
                }
                else
                {
                    mobsWithDelays.Add(waveElements[waveElementIndex]);
                    waveElementIndex++;
                }
            }

            return mobsWithDelays;
        }
        
        private static void PrepareWaveElements(WaveParametersWithChances nextWave)
        {
            _waveElements.Clear();
            
            // add all elements
            foreach (WaveElementChance waveElement in nextWave.Elements)
            {
                for (int i = 0; i < waveElement.MaxCount; i++)
                {
                    _waveElements.Add(waveElement.MobId);
                }
            }
            
            CollectionsExtensions.ShuffleList(ref _waveElements);
        }

        private static void PrepareRandomIndexes(List<WaveElementDelay> waveElements,
            List<WaveElementDelay> reinforcementElements)
        {
            _randomIndexesShuffledList.Clear();
            _randomIndexesOfReinforcements.Clear();
            byte waveWithReinforcementsLength = (byte)(waveElements.Count + reinforcementElements.Count);

            // get required amount of shuffled indexes
            for (byte i = 0; i < waveWithReinforcementsLength; i++)
            {
                _randomIndexesShuffledList.Add(i);
            }
            CollectionsExtensions.ShuffleList(ref _randomIndexesShuffledList);

            // fill hashset with random indexes
            for (byte index = 0; index < reinforcementElements.Count; index++)
            {
                _randomIndexesOfReinforcements.Add(_randomIndexesShuffledList[index]);
            }
        }
        
        private static float GetRandomWavePause(WaveParametersWithChances wave)
        {
            return Randomizer.GetRandomInRange(wave.MinSpawnPause, wave.MaxSpawnPause);
        }
        
        private static float GetRandomReinforcementPause(WaveParametersWithChances wave)
        {
            return Randomizer.GetRandomInRange(wave.MinSpawnPause, wave.MaxSpawnPause);
        }
        
        // private float GetRandomPauseInWaveDurationBounds()
        // {
        //     float waveDurationLeft = _currentWave.Duration - _currentWaveDuration;
        //     int waveElementsLeft = _currentWave.Size - _currentSpawnCount;
        //     float maxPause = Mathf.Min(_currentWave.MaxSpawnPause,
        //         // in case of emergency all left elements can be spawned with min pause
        //         waveDurationLeft - waveElementsLeft * _currentWave.MinSpawnPause);
        //     return Randomizer.GetRandomInRange(_currentWave.MinSpawnPause, maxPause);
        // }
    }
}