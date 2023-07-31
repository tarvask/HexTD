using System;
using HexSystem;
using Tools;
using UI.ScreenSpaceOverlaySystem;
using UnityEngine;

namespace Match.Field.Shooting
{
    public abstract class BaseTargetEntity : BaseDisposable, ITarget
    {
        public abstract int TargetId { get; }
        public EntityBuffableValueType EntityBuffableValueType { get; }

        public abstract Hex2d HexPosition { get; }
        public abstract Vector3 Position { get; }
        public abstract BaseReactiveModel BaseReactiveModel { get; }

        public abstract void Heal(float heal);

        public abstract void Hurt(float damage);

        public ITarget Value => this;
        public bool HasValue => true;
        public abstract ITargetView TargetView { get; }
        public IDisposable Subscribe(IObserver<ITarget> observer) => throw new NotImplementedException();
    }
}