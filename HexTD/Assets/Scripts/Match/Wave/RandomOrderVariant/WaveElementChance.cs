using System;
using UnityEngine;

namespace Match.Wave
{
    [Serializable]
    public struct WaveElementChance
    {
        [SerializeField] private byte mobId;
        //[SerializeField] private RaceType mobRace;
        //[SerializeField] private PowerType mobPower;
        [SerializeField] private byte maxCount;

        public byte MobId => mobId;
        //public RaceType MobRace => mobRace;
        //public PowerType MobPower => mobPower;
        public byte MaxCount => maxCount;

        public WaveElementChance(byte mobIdParam, byte maxCountParam)
        {
            mobId = mobIdParam;
            maxCount = maxCountParam;
        }
    }
}