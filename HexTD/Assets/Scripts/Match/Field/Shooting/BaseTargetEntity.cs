using System;
using System.Collections.Generic;
using BuffLogic;
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

        public void UpdateAddBuff(IEnumerable<IBuff> buffs, IBuff addedBuff)
        {
            ((IBuff<ITarget>)addedBuff).ApplyBuff(this);
        }

        public void UpdateRemoveBuffs(IEnumerable<IBuff> buffs, IEnumerable<IBuff> removedBuffs)
        {
            foreach (var removedBuff in removedBuffs)
            {
                ((IBuff<ITarget>)removedBuff).ApplyBuff(this);
            }
        }

        public void SubscribeOnDispose(Action<IBuffableValue> onDispose)
        {
            throw new NotImplementedException();
        }

        public ITarget Value => throw new NotImplementedException();
        public bool HasValue => throw new NotImplementedException();
        public abstract ITargetView TargetView { get; }
        public IDisposable Subscribe(IObserver<ITarget> observer) => throw new NotImplementedException();
    }
}