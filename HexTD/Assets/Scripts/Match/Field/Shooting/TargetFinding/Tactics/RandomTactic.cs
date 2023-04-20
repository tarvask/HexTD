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

        public int GetTargetWithTactic(IReadOnlyList<ITarget> targets)
        {
            if (targets.Count == 0)
                return -1;
            
            var randomTarget = targets[Randomizer.GetRandomInRange(0, targets.Count)];
            return randomTarget.TargetId;
        }
    }
}