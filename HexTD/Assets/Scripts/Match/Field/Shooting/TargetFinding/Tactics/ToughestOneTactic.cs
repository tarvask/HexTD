using System.Collections.Generic;
using Match.Field.Mob;
using Tools;

namespace Match.Field.Shooting.TargetFinding.Tactics
{
    public class ToughestOneTactic : BaseDisposable, ITargetFindingTactic
    {
        public TargetFindingTacticType TacticType => TargetFindingTacticType.ToughestOne;

        public int GetTargetWithTactic(Dictionary<int, MobController> mobs)
        {
            float highestHealth = 0;
            int mobWithHighestHealthId = -1;
            float pathOfMobWithHighestHealth = 0;

            foreach (KeyValuePair<int, MobController> mobPair in mobs)
            {
                float healthDelta = mobPair.Value.Health.Value - highestHealth;

                if (healthDelta > 0
                    || healthDelta == 0 && mobPair.Value.PathLength > pathOfMobWithHighestHealth)
                {
                    highestHealth = mobPair.Value.Health.Value;
                    pathOfMobWithHighestHealth = mobPair.Value.PathLength;
                    mobWithHighestHealthId = mobPair.Value.TargetId;
                }
            }

            return mobWithHighestHealthId;
        }
    }
}