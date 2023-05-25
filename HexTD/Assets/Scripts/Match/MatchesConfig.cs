using System;
using Tools;
using UnityEngine;

namespace Match
{
    [CreateAssetMenu(menuName = "Configs/Levels Config")]
    public class MatchesConfig : ScriptableObject
    {
        [SerializeField] private MatchConfigsDictionary levels;

        public UnitySerializedDictionary<byte, MatchConfig> Levels => levels;
    }
    
    [Serializable]
    public class MatchConfigsDictionary : UnitySerializedDictionary<byte, MatchConfig>
    {
    }
}