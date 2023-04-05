using System;
using UnityEngine;

namespace Match.Wave
{
    [Serializable]
    public struct WaveElementDelay
    {
        [SerializeField] private byte mobId;
        [SerializeField] private float delay;

        public byte MobId => mobId;
        public float Delay => delay;

        public WaveElementDelay(byte mobIdParam, float delayParam)
        {
            mobId = mobIdParam;
            delay = delayParam;
        }
    }
}