using System.Collections.Generic;
using Match.Field.Mob;
using Tools;

namespace Match.Field.Shooting.TargetFinding.Tactics
{
    public class LowestHealthTactic : BaseDisposable, ITargetFindingTactic
    {
        public TargetFindingTacticType TacticType => TargetFindingTacticType.LowestHealth;

        public int GetTargetWithTactic(IReadOnlyList<ITarget> targets)
        {
            float lowestHealth = 0;
            int mobWithLowestHealthId = -1;
            float pathOfMobWithLowestHealth = 0;

            foreach (ITarget target in targets)
            {
                float healthPart = target.BaseReactiveModel.Health.Value.CurrentValue / target.BaseReactiveModel.Health.Value.Value;
                float healthDelta = lowestHealth - healthPart;
                MobController mobController = (MobController)target;
                
                if (healthDelta > 0
                    || healthDelta == 0 && mobController.PathLength > pathOfMobWithLowestHealth)
                {
                    lowestHealth = healthPart;
                    pathOfMobWithLowestHealth = mobController.PathLength;
                    mobWithLowestHealthId = target.TargetId;
                }
            }

            return mobWithLowestHealthId;
        }
    }
}