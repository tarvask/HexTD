using HexSystem;
using UnityEngine;

namespace Match.Field.Shooting
{
    public interface IShootable
    {
        int TargetId { get; }
        Vector3 Position { get; }
        Hex2d HexPosition { get; }
        int Health { get; }
        bool IsCarrion { get; }
        void Hurt(int damage);
        //void ApplyBuffs(List<AbstractBuffParameters> buffs);
        void Die();
        void RemoveBody();
        bool CanMove { get; }
    }
}