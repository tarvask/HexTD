using Match.Wave;
using UnityEngine;

namespace Match
{
    [CreateAssetMenu(menuName = "Configs/Match/Level Config")]
    public class MatchConfig : ScriptableObject
    {
        [SerializeField] private WaveParametersStrictConfig[] waves;
        //[SerializeField] private int coinsCount;
        [SerializeField] private int energyStartCount;

        public WaveParametersStrictConfig[] WavesConfigs
        {
            get { return waves; }
#if UNITY_EDITOR
            set { waves = value; }
#endif
        }
        
        public WaveParametersStrict[] Waves
        {
            get
            {
                WaveParametersStrict[] wavesArray = new WaveParametersStrict[waves.Length];

                for (int waveIndex = 0; waveIndex < waves.Length; waveIndex++)
                {
                    wavesArray[waveIndex] = waves[waveIndex].WaveParameters;
                }
                
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
            get { return energyStartCount; }
#if UNITY_EDITOR
            set { energyStartCount = value; }
#endif
        }

#if UNITY_EDITOR
        [ContextMenu("Check Consistency")]
        private void CheckConsistency()
        {
            foreach (WaveParametersStrictConfig wave in waves)
                wave.WaveParameters.CheckConsistency();
        }
#endif
    }
}