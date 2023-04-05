using System.Collections.Generic;
using Match.Field.Mob;
using Tools;

namespace Match.Field.Shooting.TargetFinding.Tactics
{
    public class FirstInLineTactic : BaseDisposable, ITargetFindingTactic
    {
        public TargetFindingTacticType TacticType => TargetFindingTacticType.FirstInLine;
        
        public int GetTargetWithTactic(Dictionary<int, MobController> mobs)
        {
            float longestPath = 0;
            int mobWithLongestPathId = -1;
            
            foreach (KeyValuePair<int, MobController> mobPair in mobs)
            {
                if (mobPair.Value.PathLength > longestPath)
                {
                    longestPath = mobPair.Value.PathLength;
                    mobWithLongestPathId = mobPair.Value.TargetId;
                }
            }

            return mobWithLongestPathId;
        }
    }
}