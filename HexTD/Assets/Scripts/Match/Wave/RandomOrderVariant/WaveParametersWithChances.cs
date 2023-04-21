using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Match.Wave
{
    [Serializable]
    public class WaveParametersWithChances
    {
        [SerializeField] private byte size;
        [SerializeField] private float duration;
        [SerializeField] private float minSpawnPause;
        [SerializeField] private float maxSpawnPause;
        [SerializeField] private float pauseBeforeWave;
        
        [SerializeField] private WaveElementChance[] elements;
        
        public byte Size => size;
        public float Duration => duration;
        public float MinSpawnPause => minSpawnPause;
        public float MaxSpawnPause => maxSpawnPause;
        public float PauseBeforeWave => pauseBeforeWave;

        public WaveElementChance[] Elements => elements;

        public Dictionary<byte, byte> ElementsNetwork
        {
            get
            {
                Dictionary<byte, byte> elementsNetwork = new Dictionary<byte, byte>(elements.Length);

                foreach (WaveElementChance waveElement in elements)
                {
                    elementsNetwork.Add(waveElement.MobId, waveElement.MaxCount);
                }

                return elementsNetwork;
            }
        }

        public WaveParametersWithChances()
        {
            
        }

        public WaveParametersWithChances(byte sizeParam, float durationParam,
            float minSpawnPauseParam, float maxSpawnPauseParam, 
            float pauseBeforeWaveParam,
            WaveElementChance[] elementsParam)
        {
            size = sizeParam;
            duration = durationParam;
            minSpawnPause = minSpawnPauseParam;
            maxSpawnPause = maxSpawnPauseParam;
            pauseBeforeWave = pauseBeforeWaveParam;
            elements = elementsParam;
        }
        
        public Hashtable ToNetwork()
        {
            Hashtable waveNetwork = new Hashtable();
            waveNetwork[PhotonEventsConstants.SyncMatch.WaveWithRandom.SizeParam] = size;
            waveNetwork[PhotonEventsConstants.SyncMatch.WaveWithRandom.DurationParam] = duration;
            waveNetwork[PhotonEventsConstants.SyncMatch.WaveWithRandom.SpawnPauseMinParam] = minSpawnPause;
            waveNetwork[PhotonEventsConstants.SyncMatch.WaveWithRandom.SpawnPauseMaxParam] = maxSpawnPause;
            waveNetwork[PhotonEventsConstants.SyncMatch.WaveWithRandom.PauseBeforeWaveParam] = pauseBeforeWave;
            byte[] mobsIdsBytes = new byte[elements.Length];
            byte[] mobsCountsBytes = new byte[elements.Length];

            for (int elementIndex = 0; elementIndex < elements.Length; elementIndex++)
            {
                mobsIdsBytes[elementIndex] = elements[elementIndex].MobId;
                mobsCountsBytes[elementIndex] = elements[elementIndex].MaxCount;
            }

            waveNetwork[PhotonEventsConstants.SyncMatch.WaveWithRandom.MobsIdsParam] = mobsIdsBytes;
            waveNetwork[PhotonEventsConstants.SyncMatch.WaveWithRandom.MobsCountsParam] = mobsCountsBytes;

            return waveNetwork;
        }

#if UNITY_EDITOR
        [ContextMenu("Check Consistency", true)]
        public byte CheckConsistency()
        {
            byte totalMobsInWaveCount = 0;

            foreach (WaveElementChance waveElementChance in elements)
                totalMobsInWaveCount += waveElementChance.MaxCount;
            
            return size = totalMobsInWaveCount;
        }
#endif
    }
}