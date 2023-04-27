using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Match.Wave
{
    [Serializable]
    public struct WaveElementDelayAndPath
    {
        [HorizontalGroup(LabelWidth = 45)]
        [SerializeField] [MinValue(1)] [MaxValue(byte.MaxValue)] private byte mobId;
        [HorizontalGroup(LabelWidth = 45)]
        [SerializeField] private float delay;
        [HorizontalGroup(LabelWidth = 45)]
        [SerializeField] [MinValue(1)] [MaxValue(byte.MaxValue)] private byte pathId;

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