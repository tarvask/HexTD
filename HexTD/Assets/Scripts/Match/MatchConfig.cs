using Match.Field;
using Match.Wave;
using UnityEngine;

namespace Match
{
    [CreateAssetMenu(menuName = "Configs/Match/Level Config")]
    public class MatchConfig : ScriptableObject
    {
        [SerializeField] private WaveParams[] waves;
        [SerializeField] private FieldCellType[] cells;
        [SerializeField] private int silverCoinsCount;

        public WaveParams[] Waves
        {
            get { return waves; }
#if UNITY_EDITOR
            set { waves = value; }
#endif
        }
        public FieldCellType[] Cells
        {
            get { return cells; }
#if UNITY_EDITOR
            set { cells = value; }
#endif
        }

        public int SilverCoinsCount
        {
            get { return silverCoinsCount; }
#if UNITY_EDITOR
            set { silverCoinsCount = value; }
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