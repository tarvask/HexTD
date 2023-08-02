using System;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Match.Wave
{
    [Serializable]
    public class WaveParametersStrict
    {
        [SerializeField] private float pauseBeforeWave;
        [SerializeField] private WaveElementDelay[] elements;
        private float _duration;
        
        public float Duration => _duration;
        public float PauseBeforeWave => pauseBeforeWave;

        public WaveElementDelay[] Elements => elements;

        public WaveParametersStrict()
        {
            
        }
        
        public WaveParametersStrict(float durationParam,
            float pauseBeforeWaveParam,
            WaveElementDelay[] elementsParam)
        {
            _duration = durationParam;
            pauseBeforeWave = pauseBeforeWaveParam;
            elements = elementsParam;
        }
        
        public Hashtable ToNetwork()
        {
            Hashtable waveNetwork = new Hashtable();
            waveNetwork[PhotonEventsConstants.SyncMatchOnLoad.WaveStrictOrder.DurationParam] = _duration;
            waveNetwork[PhotonEventsConstants.SyncMatchOnLoad.WaveStrictOrder.PauseBeforeWaveParam] = pauseBeforeWave;
            byte[] mobsIdsBytes = new byte[elements.Length];
            float[] mobsDelaysBytes = new float[elements.Length];

            for (int elementIndex = 0; elementIndex < elements.Length; elementIndex++)
            {
                mobsIdsBytes[elementIndex] = elements[elementIndex].MobId;
                mobsDelaysBytes[elementIndex] = elements[elementIndex].Delay;
            }

            waveNetwork[PhotonEventsConstants.SyncMatchOnLoad.WaveStrictOrder.MobsIdsParam] = mobsIdsBytes;
            waveNetwork[PhotonEventsConstants.SyncMatchOnLoad.WaveStrictOrder.MobsDelaysParam] = mobsDelaysBytes;

            return waveNetwork;
        }

#if UNITY_EDITOR
        [ContextMenu("Check Consistency", true)]
        public void CheckConsistency()
        {
            float totalWaveDuration = 0;

            foreach (WaveElementDelay waveElementDelay in elements)
            {
                if (waveElementDelay.Delay > totalWaveDuration)
                    totalWaveDuration = waveElementDelay.Delay;
            }
            
            _duration = totalWaveDuration;
        }
#endif
    }
}