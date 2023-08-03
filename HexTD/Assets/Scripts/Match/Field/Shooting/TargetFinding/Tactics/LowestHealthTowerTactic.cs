using System.Collections.Generic;
using Tools;

namespace Match.Field.Shooting.TargetFinding.Tactics
{
    public class LowestHealthTowerTactic : BaseDisposable, ITargetFindingTactic
    {
        public TargetFindingTacticType TacticType => TargetFindingTacticType.LowestHealthTower;

        public int GetTargetWithTactic(IReadOnlyList<ITarget> targets)
        {
            float lowestHealth = float.MaxValue;
            int towerWithLowestHealthId = -1;

            foreach (ITarget target in targets)
            {
                // do not aim at targets with full health 
                if (target.BaseReactiveModel.Health.Value.CurrentValue >= target.BaseReactiveModel.Health.Value.Value)
                    continue;
                
                float healthPart = target.BaseReactiveModel.Health.Value.CurrentValue / target.BaseReactiveModel.Health.Value.Value;
                float healthDelta = lowestHealth - healthPart;

                if (healthDelta > 0)
                {
                    lowestHealth = target.BaseReactiveModel.Health.Value.CurrentValue;
                    towerWithLowestHealthId = target.TargetId;
                }
            }

            return towerWithLowestHealthId;
        }
    }
}