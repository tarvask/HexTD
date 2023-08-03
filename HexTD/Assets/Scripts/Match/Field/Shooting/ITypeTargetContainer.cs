using System.Collections.Generic;
using Match.Serialization;

namespace Match.Field.Shooting
{
    public interface ITypeTargetContainer : IEnumerable<ITarget>, ISerializableToNetwork
    {
        // targets by positions, it's convenient to use
        IReadOnlyDictionary<int, List<ITarget>> TargetsByPosition { get; }
    }
}