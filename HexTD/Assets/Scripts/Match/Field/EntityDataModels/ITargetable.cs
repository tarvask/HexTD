using BuffLogic;
using UnityEngine;

namespace Match.Field.Shooting
{
    public interface ITargetable : IBuffableValue<ITargetable>
    {
        int TargetId { get; }
        Vector3 Position { get; }
        BaseReactiveModel BaseReactiveModel { get; }
        void Hurt(float damage);
    }
}