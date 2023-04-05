using Match.Field.Buff;

namespace Match.Field.Tower
{
    public interface IAbility
    {
        string AbilityName { get; }
        bool IsConditional { get; }
        bool IsSelfApplied { get; }
        bool IsTargetApplied { get; }
        bool IsTowersInAreaApplied { get; }
        AbstractBuffParameters AbilityToBuff();
    }
}