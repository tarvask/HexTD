using System;
using Match.Field.Buff;

namespace Match.Field.Tower
{
    [Serializable]
    public abstract class AbstractAbilityMarker : AbstractMarker, IAbility
    {
        public abstract string AbilityName { get; }
        public abstract BuffedParameterType BuffedParameterType { get; }
        public abstract AbstractBuffParameters AbilityToBuff();
        public abstract bool IsConditional { get; }
        public abstract bool IsSelfApplied { get; }
        public abstract bool IsTargetApplied { get; }
        public abstract bool IsTowersInAreaApplied { get; }
    }
}