using Match.Wave;
using UnityEngine;

namespace Match
{
    [CreateAssetMenu(menuName = "Configs/Match/Level Config")]
    public class MatchConfig : ScriptableObject
    {
        [SerializeField] private WaveParametersStrict[] waves;
        //[SerializeField] private int coinsCount;
        [SerializeField] private int energyStartCount;

        public WaveParametersStrict[] Waves
        {
            get { return waves; }
#if UNITY_EDITOR
            set { waves = value; }
#endif
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
            foreach (WaveParametersStrict wave in waves)
                wave.CheckConsistency();
        }
#endif
    }
}