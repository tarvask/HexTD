using UniRx;

namespace Match.Field.Mob
{
    public class MobBuffsManager //: BaseBuffsManager
    {
        public struct Context
        {
            public IReadOnlyReactiveProperty<float> BaseMovementSpeed { get; }
            public IReadOnlyReactiveProperty<int> BaseHealth { get; }

            public Context(IReadOnlyReactiveProperty<float> baseMovementSpeed, IReadOnlyReactiveProperty<int> baseHealth)
            {
                BaseMovementSpeed = baseMovementSpeed;
                BaseHealth = baseHealth;
            }
        }

        private readonly Context _context;

        public MobBuffsManager(Context context)
        {
            _context = context;

            // _buffedParametersByTypes.Add((byte)BuffedParameterType.MovementSpeed,
            //     new MovementSpeedBuffedParameter(BuffedParameterType.MovementSpeed,
            //         _context.BaseMovementSpeed, 0, float.MaxValue));
            // _buffedParametersByTypes.Add((byte)BuffedParameterType.Health,
            //     new HealthBuffedParameter(BuffedParameterType.Health,
            //         0, int.MinValue, int.MaxValue));
        }
    }
}