using System;
using Match.Wave;
using UnityEngine;

namespace Match
{
    [CreateAssetMenu(menuName = "Configs/Match/Level Config")]
    public class MatchConfig : ScriptableObject
    {
        [SerializeField] private PathWithWaves[] pathsWithWaves;
        //[SerializeField] private int coinsCount;
        [SerializeField] private int initialEnergy;
        [SerializeField] private float energyRecoveryTime;
        public const int EnergyRestoreValue = 1;
        public const int EnergyMaxCount = 100000;
        [SerializeField] private TextAsset levelMap;

        public PathWithWaves[] WavesConfigs
        {
            get { return pathsWithWaves; }
#if UNITY_EDITOR
            set { pathsWithWaves = value; }
#endif
        }
        
        public WaveWithDelayAndPath[] Waves
        {
            get
            {
                int wavesTotalCount = 0;
                
                foreach (PathWithWaves path in pathsWithWaves)
                    wavesTotalCount += path.Waves.Length;
                
                WaveWithDelayAndPath[] wavesArray = new WaveWithDelayAndPath[wavesTotalCount];
                int waveIndex = 0;

                foreach (PathWithWaves path in pathsWithWaves)
                {
                    foreach (WaveWithDelay waveWithDelay in path.Waves)
                    {
                        wavesArray[waveIndex++] = new WaveWithDelayAndPath(waveWithDelay.WaveConfig.WaveParameters,
                            waveWithDelay.WaveDelay,
                            path.PathId);
                    }
                }
                
                Array.Sort(wavesArray, WaveWithDelayAndPathComparer);
                
                return wavesArray;
            }
        }

//         public int CoinsCount
//         {
//             get { return coinsCount; }
// #if UNITY_EDITOR
//             set { coinsCount = value; }
// #endif
//         }

        public int EnergyStartCount
        {
            get { return initialEnergy; }
#if UNITY_EDITOR
            set { initialEnergy = value; }
#endif
        }

        public float EnergyRestoreDelay
        {
            get { return energyRecoveryTime; }
#if UNITY_EDITOR
            set { energyRecoveryTime = value; }
#endif
        }

        public TextAsset LevelMap
        {
            get { return levelMap; }
#if UNITY_EDITOR
            set { levelMap = value; }
#endif
        }

        private static int WaveWithDelayAndPathComparer(WaveWithDelayAndPath wave1, WaveWithDelayAndPath wave2)
        {
            int result = wave1.WaveDelay.CompareTo(wave2.WaveDelay);
            
            if (result != 0)
                return result;

            result = wave1.PathId.CompareTo(wave2.PathId);

            return result;
        }

#if UNITY_EDITOR
        [ContextMenu("Check Consistency")]
        private void CheckConsistency()
        {
            foreach (PathWithWaves path in pathsWithWaves)
            {
                foreach (WaveWithDelay wave in path.Waves)
                    wave.WaveConfig.WaveParameters.CheckConsistency();
            }
        }
#endif
    }
}