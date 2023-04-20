using System.Collections.Generic;

namespace Match.Field.Shooting
{
    public interface ITypeTargetContainer : IEnumerable<ITarget>
    {
        // targets by positions, it's convenient to use
        IReadOnlyDictionary<int, List<ITarget>> TargetsByPosition { get; }
    }
}