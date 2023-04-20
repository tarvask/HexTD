using System.Collections;
using System.Collections.Generic;

namespace Match.Field.Shooting
{
    //TODO: add mobs
    public class ShooterContainer : IEnumerable<IShooter>
    {
        private static EnumAttackerType[] _iteratesAttackerTypes = new[]
        {
            EnumAttackerType.Tower
        };
        
        private readonly Dictionary<EnumAttackerType, IShooterContainer> _targetsIterators;

        //TODO: add mobs
        public ShooterContainer(IShooterContainer towers)
        {
            _targetsIterators = new Dictionary<EnumAttackerType, IShooterContainer>(2);
            _targetsIterators.Add(EnumAttackerType.Tower, towers);
        }

        public IEnumerator<IShooter> GetEnumerator()
        {
            foreach (var attackTargetType in _iteratesAttackerTypes)
            {
                IShooterContainer shooterContainer = _targetsIterators[attackTargetType];

                foreach (var shooter in shooterContainer)
                {
                    yield return shooter;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}