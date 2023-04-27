using UnityEngine;

namespace Match.Wave
{
    [CreateAssetMenu(menuName = "Configs/Match/Wave Config")]
    public class WaveParametersStrictConfig : ScriptableObject
    {
        [SerializeField] private WaveParametersStrict waveParameters;

        public WaveParametersStrict WaveParameters
        {
            get { return waveParameters; }
#if UNITY_EDITOR
            set { waveParameters = value; }
#endif
        }

        
#if UNITY_EDITOR
        private void OnValidate()
        {
            waveParameters.CheckConsistency();
        }
#endif
    }
}