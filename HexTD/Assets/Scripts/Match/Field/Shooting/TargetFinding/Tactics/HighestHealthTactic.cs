using System.Collections.Generic;
using Match.Field.Mob;
using Tools;

namespace Match.Field.Shooting.TargetFinding.Tactics
{
    public class HighestHealthTactic : BaseDisposable, ITargetFindingTactic
    {
        public TargetFindingTacticType TacticType => TargetFindingTacticType.HighestHealth;

        public int GetTargetWithTactic(IReadOnlyList<ITarget> targets)
        {
            float highestHealth = 0;
            int mobWithHighestHealthId = -1;
            float pathOfMobWithHighestHealth = 0;

            foreach (ITarget target in targets)
            {
                float healthPart = target.BaseReactiveModel.Health.Value.CurrentValue / target.BaseReactiveModel.Health.Value.Value;
                float healthDelta = healthPart - highestHealth;
                MobController mobController = (MobController)target;

                if (healthDelta > 0
                    || healthDelta == 0 && mobController.PathLength > pathOfMobWithHighestHealth)
                {
                    highestHealth = healthPart;
                    pathOfMobWithHighestHealth = mobController.PathLength;
                    mobWithHighestHealthId = target.TargetId;
                }
            }

            return mobWithHighestHealthId;
        }
    }
}