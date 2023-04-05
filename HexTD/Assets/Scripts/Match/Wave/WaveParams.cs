using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Match.Wave
{
    [Serializable]
    public class WaveParams
    {
        [SerializeField] private byte size;
        [SerializeField] private float duration;
        [SerializeField] private float minSpawnPause;
        [SerializeField] private float maxSpawnPause;
        [SerializeField] private bool areArtifactsAvailable;
        [SerializeField] private float pauseBeforeWave;
        
        [SerializeField] private WaveElementChance[] elements;
        
        public byte Size => size;
        public float Duration => duration;
        public float MinSpawnPause => minSpawnPause;
        public float MaxSpawnPause => maxSpawnPause;
        public bool AreArtifactsAvailable => areArtifactsAvailable;
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

        public WaveParams()
        {
            
        }

        public WaveParams(byte sizeParam, float durationParam,
            float minSpawnPauseParam, float maxSpawnPauseParam,
            bool areArtifactsAvailableParam, float pauseBeforeWaveParam,
            WaveElementChance[] elementsParam)
        {
            size = sizeParam;
            duration = durationParam;
            minSpawnPause = minSpawnPauseParam;
            maxSpawnPause = maxSpawnPauseParam;
            areArtifactsAvailable = areArtifactsAvailableParam;
            pauseBeforeWave = pauseBeforeWaveParam;
            elements = elementsParam;
        }

        public Hashtable ToNetwork()
        {
            Hashtable waveNetwork = new Hashtable();
            waveNetwork[PhotonEventsConstants.SyncMatch.Wave.SizeParam] = size;
            waveNetwork[PhotonEventsConstants.SyncMatch.Wave.DurationParam] = duration;
            waveNetwork[PhotonEventsConstants.SyncMatch.Wave.SpawnPauseMinParam] = minSpawnPause;
            waveNetwork[PhotonEventsConstants.SyncMatch.Wave.SpawnPauseMaxParam] = maxSpawnPause;
            waveNetwork[PhotonEventsConstants.SyncMatch.Wave.ArtifactsParam] = areArtifactsAvailable;
            waveNetwork[PhotonEventsConstants.SyncMatch.Wave.PauseBeforeWaveParam] = pauseBeforeWave;
            byte[] mobsIdsBytes = new byte[elements.Length];
            byte[] mobsCountsBytes = new byte[elements.Length];

            for (int elementIndex = 0; elementIndex < elements.Length; elementIndex++)
            {
                mobsIdsBytes[elementIndex] = elements[elementIndex].MobId;
                mobsCountsBytes[elementIndex] = elements[elementIndex].MobId;
            }

            waveNetwork[PhotonEventsConstants.SyncMatch.Wave.MobsIdsParam] = mobsIdsBytes;
            waveNetwork[PhotonEventsConstants.SyncMatch.Wave.MobsCountsParam] = mobsCountsBytes;

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