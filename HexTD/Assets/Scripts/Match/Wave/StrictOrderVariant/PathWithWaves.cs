using System;
using UnityEngine;

namespace Match.Wave
{
    [Serializable]
    public class PathWithWaves
    {
        [SerializeField] private byte pathId;
        [SerializeField] private WaveWithDelay[] waves;

        public byte PathId => pathId;
        public WaveWithDelay[] Waves => waves;
    }
}