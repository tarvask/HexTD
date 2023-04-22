using System.Collections.Generic;
using Match.Field.Mob;
using Tools;

namespace Match.Field.Shooting.TargetFinding.Tactics
{
    public class FirstInLineTactic : BaseDisposable, ITargetFindingTactic
    {
        public TargetFindingTacticType TacticType => TargetFindingTacticType.FirstInLine;
        
        public int GetTargetWithTactic(IReadOnlyList<ITarget> targets)
        {
            float shorterPath = float.MaxValue;
            int mobWithLongestPathId = -1;
            
            foreach (var target in targets)
            {
                MobController mobController = (MobController)target;
                if (mobController.RemainingPathDistance < shorterPath)
                {
                    shorterPath = mobController.RemainingPathDistance;
                    mobWithLongestPathId = mobController.TargetId;
                }
            }

            return mobWithLongestPathId;
        }
    }
}