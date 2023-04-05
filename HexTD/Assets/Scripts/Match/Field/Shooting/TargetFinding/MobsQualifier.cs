using System.Collections.Generic;
using Match.Field.Mob;
using Match.Wave;
using Tools;

namespace Match.Field.Shooting.TargetFinding
{
    public class MobsQualifier : BaseDisposable
    {
        private readonly Dictionary<int, MobController> _qualifyingMobs;

        public MobsQualifier()
        {
            _qualifyingMobs = new Dictionary<int, MobController>(WaveMobSpawnerCoordinator.MaxMobsInWave);
        }
        
        public Dictionary<int, MobController> GetMobsWithoutBuffs(Dictionary<int, MobController> mobs, bool preferUnbuffed)//,
            //List<AbstractBuffParameters> towerActiveBuffs)
        {
            //if (!preferUnbuffed || towerActiveBuffs == null || towerActiveBuffs.Count == 0)
              //  return mobs;
            
            _qualifyingMobs.Clear();
            
            // query mobs that do not have at least one buff from tower
            foreach (KeyValuePair<int, MobController> mobPair in mobs)
            {
                // foreach (AbstractBuffParameters towerBuff in towerActiveBuffs)
                // {
                //     if (!mobPair.Value.HasBuff(towerBuff))
                //     {
                //         _qualifyingMobs.Add(mobPair.Key, mobPair.Value);
                //         break;
                //     }
                // }
                _qualifyingMobs.Add(mobPair.Key, mobPair.Value);
            }

            if (_qualifyingMobs.Count == 0)
                return mobs;
            
            return _qualifyingMobs;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _qualifyingMobs.Clear();
        }
    }
}