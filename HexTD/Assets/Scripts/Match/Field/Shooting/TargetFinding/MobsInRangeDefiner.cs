using System.Collections.Generic;
using Match.Field.Mob;
using Match.Wave;
using Tools;
using UnityEngine;

namespace Match.Field.Shooting.TargetFinding
{
    public class MobsInRangeDefiner : BaseDisposable
    {
        private readonly Dictionary<int, MobController> _mobsInRange;

        public MobsInRangeDefiner()
        {
            _mobsInRange = new Dictionary<int, MobController>(WaveMobSpawnerCoordinator.MaxMobsInWave);
        }
        
        public Dictionary<int, MobController> GetTargetsInRange(Dictionary<int, MobController> mobs,
            Vector3 towerPosition, float attackRadiusSqr)
        {
            _mobsInRange.Clear();

            foreach (KeyValuePair<int, MobController> mobPair in mobs)
                if ((mobPair.Value.Position - towerPosition).sqrMagnitude < attackRadiusSqr)
                    _mobsInRange.Add(mobPair.Key, mobPair.Value);

            return _mobsInRange;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _mobsInRange.Clear();
        }
    }
}