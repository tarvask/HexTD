using System;
using UnityEngine;

namespace Match.Wave
{
    [Serializable]
    public class WaveWithDelay
    {
        [SerializeField] private float waveDelay;
        [SerializeField] private WaveParametersStrictConfig waveConfig;

        public float WaveDelay => waveDelay;
        public WaveParametersStrictConfig WaveConfig => waveConfig;
    }
}