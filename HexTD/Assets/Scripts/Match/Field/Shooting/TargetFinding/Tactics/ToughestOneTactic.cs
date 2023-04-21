using System.Collections.Generic;
using Match.Field.Mob;
using Tools;

namespace Match.Field.Shooting.TargetFinding.Tactics
{
    public class ToughestOneTactic : BaseDisposable, ITargetFindingTactic
    {
        public TargetFindingTacticType TacticType => TargetFindingTacticType.ToughestOne;

        public int GetTargetWithTactic(IReadOnlyList<ITarget> targets)
        {
            float highestHealth = 0;
            int mobWithHighestHealthId = -1;
            float pathOfMobWithHighestHealth = 0;

            foreach (ITarget target in targets)
            {
                MobController mobController = (MobController)target;
                float healthDelta = target.BaseReactiveModel.Health.Value - highestHealth;

                if (healthDelta > 0
                    || healthDelta == 0 && mobController.PathLength > pathOfMobWithHighestHealth)
                {
                    highestHealth = target.BaseReactiveModel.Health.Value;
                    pathOfMobWithHighestHealth = mobController.PathLength;
                    mobWithHighestHealthId = target.TargetId;
                }
            }

            return mobWithHighestHealthId;
        }
    }
}