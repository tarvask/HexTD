using Match.Field.Buff;
using UniRx;

namespace Match.Field.Tower
{
    public class TowerBuffsManager : BaseBuffsManager
    {
        public struct Context
        {
            public IReadOnlyReactiveProperty<float> BaseAttackPower { get; }
            public IReadOnlyReactiveProperty<float> BaseAttackRadius { get; }
            public IReadOnlyReactiveProperty<float> BaseReloadTime { get; }

            public Context(IReadOnlyReactiveProperty<float> baseAttackPower,
                IReadOnlyReactiveProperty<float> baseAttackRadius,
                IReadOnlyReactiveProperty<float> baseReloadTime)
            {
                BaseAttackPower = baseAttackPower;
                BaseAttackRadius = baseAttackRadius;
                BaseReloadTime = baseReloadTime;
            }
        }

        private readonly Context _context;

        public TowerBuffsManager(Context context)
        {
            _context = context;
            
            // _buffedParametersByTypes.Add((byte)BuffedParameterType.AttackPower,
            //     new AttackPowerBuffedParameter(BuffedParameterType.AttackPower,
            //         _context.BaseAttackPower, 0, float.MaxValue));
            // _buffedParametersByTypes.Add((byte)BuffedParameterType.AttackRadius,
            //     new AttackRadiusBuffedParameter(BuffedParameterType.AttackRadius,
            //         _context.BaseAttackRadius, 0, float.MaxValue));
            // _buffedParametersByTypes.Add((byte)BuffedParameterType.ReloadTime,
            //     new ReloadTimeBuffedParameter(BuffedParameterType.ReloadTime,
            //         _context.BaseReloadTime, 0, float.MaxValue));
        }
    }
}