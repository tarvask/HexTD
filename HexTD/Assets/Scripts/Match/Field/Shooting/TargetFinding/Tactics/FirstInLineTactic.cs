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
            float shorterPath = float.MaxValue;
            int mobWithLongestPathId = -1;
            
            foreach (KeyValuePair<int, MobController> mobPair in mobs)
            {
                if (mobPair.Value.RemainingPathDistance < shorterPath)
                {
                    shorterPath = mobPair.Value.RemainingPathDistance;
                    mobWithLongestPathId = mobPair.Value.TargetId;
                }
            }

            return mobWithLongestPathId;
        }
    }
}