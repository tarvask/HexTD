using BuffLogic;
using HexSystem;
using UnityEngine;

namespace Match.Field.Shooting
{
    public interface IShootable : IBuffableValue<IShootable>
    {
        int TargetId { get; }
        Vector3 Position { get; }
        Hex2d HexPosition { get; }
        IReadonlyBuffableValue<float> Health { get; }
        IReadonlyBuffableValue<float> Speed { get; }
        bool IsCarrion { get; }
        void Hurt(float damage);
        void Die();
        void RemoveBody();
        bool CanMove { get; }
    }
}