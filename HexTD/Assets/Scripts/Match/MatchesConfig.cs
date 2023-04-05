using UnityEngine;

namespace Match
{
    [CreateAssetMenu(menuName = "Configs/Levels Config")]
    public class MatchesConfig : ScriptableObject
    {
        [SerializeField] private MatchConfig[] levels;

        public MatchConfig[] Levels => levels;
    }
}