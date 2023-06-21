using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FarmDataBase
{
    public class FarmDataBase : MonoBehaviour
    {
        [Serializable]
        public class Settings
        {
            public FarmSettings FarmSettings;
        }

        private readonly Settings _settings;

        public FarmDataBase(Settings settings)
        {
            _settings = settings;
        }

        public FarmSettings FarmSettings => _settings.FarmSettings;
    }
    [Serializable]
    public class FarmSettings
    {
        public AssetReferenceGameObject FarmPrefabReference;
        public AssetLabelReference FarmLabel;
    }
}