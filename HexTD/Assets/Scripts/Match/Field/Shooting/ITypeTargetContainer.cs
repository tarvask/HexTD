using System.Collections.Generic;

namespace Match.Field.Shooting
{
    public interface ITypeTargetContainer : IEnumerable<ITargetable>
    {
        // targets by positions, it's convenient to use
        IReadOnlyDictionary<int, List<ITargetable>> TargetsByPosition { get; }
    }
}