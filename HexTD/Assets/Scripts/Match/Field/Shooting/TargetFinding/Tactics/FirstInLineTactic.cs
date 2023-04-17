using System.Collections.Generic;
using Match.Field.Mob;
using Tools;

namespace Match.Field.Shooting.TargetFinding.Tactics
{
    public class FirstInLineTactic : BaseDisposable, ITargetFindingTactic
    {
        public TargetFindingTacticType TacticType => TargetFindingTacticType.FirstInLine;
        
        public int GetTargetWithTactic(IReadOnlyList<ITargetable> targets)
        {
            float longestPath = 0;
            int mobWithLongestPathId = -1;
            
            foreach (var target in targets)
            {
                MobController mobController = (MobController)target;
                if (mobController.PathLength > longestPath)
                {
                    longestPath = mobController.PathLength;
                    mobWithLongestPathId = mobController.TargetId;
                }
            }

            return mobWithLongestPathId;
        }
    }
}