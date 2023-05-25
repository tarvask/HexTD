using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Match.Wave
{
    [Serializable]
    public struct WaveElementDelay
    {
        [HorizontalGroup(LabelWidth = 45)]
        [SerializeField] [MinValue(1)] [MaxValue(byte.MaxValue)] private byte mobId;
        [HorizontalGroup(LabelWidth = 45)]
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