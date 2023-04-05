using System;
using UnityEngine;

namespace Match.Field.Tower
{
    [Serializable]
    public class TowerParameters
    {
        [SerializeField] private TowerRegularParametersMarker regularParameters;
        [SerializeField] private TowerLevelParams[] levels;
        //[SerializeField] private int fragmentsCount;
        //[SerializeField] private int[] alliances;

        public TowerRegularParametersMarker RegularParameters
        {
            get => regularParameters;
#if UNITY_EDITOR
            set => regularParameters = value;
#endif
        }

        public TowerLevelParams[] Levels
        {
            get => levels;
#if UNITY_EDITOR
            set => levels = value;
#endif
        }

        public static class FieldNames
        {
            public static string RegularParameters => nameof(regularParameters);
            public static string Levels => nameof(levels);
        }
    }
}