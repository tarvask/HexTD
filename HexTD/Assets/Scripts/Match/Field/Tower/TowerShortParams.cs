using System;
using UnityEngine;

namespace Match.Field.Tower
{
    [Serializable]
    public struct TowerShortParams
    {
        [SerializeField] private TowerType towerType;
        [SerializeField] private int level;

        public TowerType TowerType => towerType;
        public int Level => level;

        public TowerShortParams(TowerType type, int lvl)
        {
            towerType = type;
            level = lvl;
        }
    }
}