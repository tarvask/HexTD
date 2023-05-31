using System.Collections.Generic;
using UI.Tools;

namespace Match.Field.Shooting
{
    public class TargetContainer : BaseDisposable
    {
        private readonly Dictionary<EnumAttackTargetType, ITypeTargetContainer> _targetsIterators;

        public TargetContainer(ITypeTargetContainer mobs, ITypeTargetContainer towers)
        {
            _targetsIterators = new Dictionary<EnumAttackTargetType, ITypeTargetContainer>(2);
            _targetsIterators.Add(EnumAttackTargetType.Mob, mobs);
            _targetsIterators.Add(EnumAttackTargetType.Tower, towers);
        }

        public bool TryGetTargetByIdAndType(int targetId, EnumAttackTargetType attackTargetType, out ITarget target)
        {
            ITypeTargetContainer targetTypedIterator = _targetsIterators[attackTargetType];

            foreach (var potentialTarget in targetTypedIterator)
            {
                if (potentialTarget.TargetId == targetId)
                {
                    target = potentialTarget;
                    return true;
                }
            }

            target = null;
            return false;
        }

        public IReadOnlyDictionary<int, List<ITarget>> GetTargetsByPosition(EnumAttackTargetType attackTargetType)
        {
            return _targetsIterators[attackTargetType].TargetsByPosition;
        }

        public IEnumerable<ITarget> IterateTargetsWithTypes(params EnumAttackTargetType[] attackTargetTypes)
        {
            foreach (var attackTargetType in attackTargetTypes)
            {
                ITypeTargetContainer targetTypedIterator = _targetsIterators[attackTargetType];

                foreach (var target in targetTypedIterator)
                {
                    yield return target;
                }
            }
        }
    }
}