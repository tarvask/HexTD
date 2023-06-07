using BuffLogic;
using HexSystem;
using UI.ScreenSpaceOverlaySystem;
using UnityEngine;

namespace Match.Field.Shooting
{
    public interface ITarget : IBuffableValue<ITarget>
    {
        int TargetId { get; }
        Hex2d HexPosition { get; }
        Vector3 Position { get; }
        BaseReactiveModel BaseReactiveModel { get; }
        void Heal(float heal);
        void Hurt(float damage);
        ITargetView TargetView { get; }
    }
}