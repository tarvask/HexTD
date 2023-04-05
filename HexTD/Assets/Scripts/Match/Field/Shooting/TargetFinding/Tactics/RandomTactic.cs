using System.Collections.Generic;
using Match.Field.Mob;
using Match.Wave;
using Tools;

namespace Match.Field.Shooting.TargetFinding.Tactics
{
    public class RandomTactic : BaseDisposable, ITargetFindingTactic
    {
        private readonly List<int> _possibleTargets;
        
        public RandomTactic()
        {
            _possibleTargets = new List<int>(WaveMobSpawnerCoordinator.MaxMobsInWave);
        }
        
        public TargetFindingTacticType TacticType => TargetFindingTacticType.Random;
        
        public int GetTargetWithTactic(Dictionary<int, MobController> mobs)
        {
            _possibleTargets.Clear();
            
            foreach (KeyValuePair<int, MobController> mobPair in mobs)
                _possibleTargets.Add(mobPair.Key);
            
            if (_possibleTargets.Count == 0)
                return -1;

            int randomMobIndex = _possibleTargets[Randomizer.GetRandomInRange(0, _possibleTargets.Count)];
            
            return mobs[randomMobIndex].TargetId;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _possibleTargets.Clear();
        }
    }
}