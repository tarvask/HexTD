using System.Collections.Generic;
using Match.Field.Mob;

namespace Match.Field.Shooting.TargetFinding.Tactics
{
    public interface ITargetFindingTactic
    {
        TargetFindingTacticType TacticType { get; }
        int GetTargetWithTactic(IReadOnlyList<ITarget> targets);
    }
}