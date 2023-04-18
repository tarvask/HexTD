using System.Collections.Generic;
using Match.Field.Mob;
using Match.Wave;
using Tools;

namespace Match.Field.Shooting.TargetFinding.Tactics
{
    public class RandomTactic : BaseDisposable, ITargetFindingTactic
    {
        public RandomTactic()
        {
        }
        
        public TargetFindingTacticType TacticType => TargetFindingTacticType.Random;

        public int GetTargetWithTactic(IReadOnlyList<ITargetable> targets)
        {
            if (targets.Count == 0)
                return -1;
            
            var randomMobIndex = targets[Randomizer.GetRandomInRange(0, targets.Count)];
            return randomMobIndex.TargetId;
        }
    }
}