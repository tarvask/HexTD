using UnityEngine;

namespace Match.Field.Mob
{
    [CreateAssetMenu(menuName = "Configs/Match/Mobs Config")]
    public class MobsConfig : ScriptableObject
    {
        [SerializeField] private MobConfig[] mobs;
        
        public MobConfig[] Mobs => mobs;
    }
}