using System;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Match.Wave
{
    [Serializable]
    public class WaveParametersStrict
    {
        [SerializeField] private byte size;
        [SerializeField] private float duration;
        [SerializeField] private float pauseBeforeWave;

        [SerializeField] private WaveElementDelay[] elements;
        
        public byte Size => size;
        public float Duration => duration;
        public float PauseBeforeWave => pauseBeforeWave;

        public WaveElementDelay[] Elements => elements;

        public WaveParametersStrict()
        {
            
        }
        
        public WaveParametersStrict(byte sizeParam, float durationParam,
            float pauseBeforeWaveParam,
            WaveElementDelay[] elementsParam)
        {
            size = sizeParam;
            duration = durationParam;
            pauseBeforeWave = pauseBeforeWaveParam;
            elements = elementsParam;
        }
        
        public Hashtable ToNetwork()
        {
            Hashtable waveNetwork = new Hashtable();
            waveNetwork[PhotonEventsConstants.SyncMatch.WaveStrictOrder.SizeParam] = size;
            waveNetwork[PhotonEventsConstants.SyncMatch.WaveStrictOrder.DurationParam] = duration;
            waveNetwork[PhotonEventsConstants.SyncMatch.WaveStrictOrder.PauseBeforeWaveParam] = pauseBeforeWave;
            byte[] mobsIdsBytes = new byte[elements.Length];
            float[] mobsDelaysBytes = new float[elements.Length];

            for (int elementIndex = 0; elementIndex < elements.Length; elementIndex++)
            {
                mobsIdsBytes[elementIndex] = elements[elementIndex].MobId;
                mobsDelaysBytes[elementIndex] = elements[elementIndex].Delay;
            }

            waveNetwork[PhotonEventsConstants.SyncMatch.WaveStrictOrder.MobsIdsParam] = mobsIdsBytes;
            waveNetwork[PhotonEventsConstants.SyncMatch.WaveStrictOrder.MobsDelaysParam] = mobsDelaysBytes;

            return waveNetwork;
        }

#if UNITY_EDITOR
        [ContextMenu("Check Consistency", true)]
        public void CheckConsistency()
        {
            byte totalMobsInWaveCount = (byte)elements.Length;
            float totalWaveDuration = 0;

            foreach (WaveElementDelay waveElementDelay in elements)
                totalWaveDuration += waveElementDelay.Delay;

            size = totalMobsInWaveCount;
            duration = totalWaveDuration;
        }
#endif
    }
}