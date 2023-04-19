using Match.Wave;
using UnityEngine;

namespace Match
{
    [CreateAssetMenu(menuName = "Configs/Match/Level Config")]
    public class MatchConfig : ScriptableObject
    {
        [SerializeField] private WaveParams[] waves;
        [SerializeField] private int coinsCount;

        public WaveParams[] Waves
        {
            get { return waves; }
#if UNITY_EDITOR
            set { waves = value; }
#endif
        }

        public int CoinsCount
        {
            get { return coinsCount; }
#if UNITY_EDITOR
            set { coinsCount = value; }
#endif
        }

#if UNITY_EDITOR
        [ContextMenu("Check Consistency")]
        private void CheckConsistency()
        {
            foreach (WaveParams wave in waves)
                wave.CheckConsistency();
        }
#endif
    }
}