using BuffLogic;
using HexSystem;
using UnityEngine;

namespace Match.Field.Shooting
{
    public interface ITargetable : IBuffableValue<ITargetable>
    {
        int TargetId { get; }
        Hex2d HexPosition { get; }
        Vector3 Position { get; }
        BaseReactiveModel BaseReactiveModel { get; }
        void Heal(float heal);
        void Hurt(float damage);
    }
}