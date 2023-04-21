using System;
using UnityEngine;

namespace Match.Wave
{
    [Serializable]
    public struct WaveElementDelayAndPath
    {
        [SerializeField] private byte mobId;
        [SerializeField] private float delay;
        [SerializeField] private byte pathId;

        public byte MobId => mobId;
        public float Delay => delay;
        public byte PathId => pathId;

        public WaveElementDelayAndPath(byte mobIdParam, float delayParam, byte pathIdParam)
        {
            mobId = mobIdParam;
            delay = delayParam;
            pathId = pathIdParam;
        }
    }
}