using UnityEngine;

namespace Match.Field.Mob
{
    [CreateAssetMenu(menuName = "Configs/Match/Mobs Config")]
    public class MobsConfig : ScriptableObject
    {
        // water
        [SerializeField] private MobConfig[] waterMobs;

        // fire
        [SerializeField] private MobConfig[] fireMobs;

        // earth
        [SerializeField] private MobConfig[] earthMobs;

        // nature
        [SerializeField] private MobConfig[] natureMobs;

        // death
        [SerializeField] private MobConfig[] deathMobs;
        
        // reinforcements
        [SerializeField] private MobConfig[] troops;
        
        // water
        public MobConfig[] WaterMobs => waterMobs;
        // fire
        public MobConfig[] FireMobs => fireMobs;
        // earth
        public MobConfig[] EarthMobs => earthMobs;
        // nature
        public MobConfig[] NatureMobs => natureMobs;
        // death
        public MobConfig[] DeathMobs => deathMobs;
        
        // reinforcements
        public MobConfig[] Troops => troops;
    }
}