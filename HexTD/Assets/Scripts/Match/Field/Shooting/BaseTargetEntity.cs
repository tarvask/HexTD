using System.Collections.Generic;
using BuffLogic;
using HexSystem;
using Tools;
using Tools.PriorityTools;
using UnityEngine;

namespace Match.Field.Shooting
{
    public abstract class BaseTargetEntity : BaseDisposable, ITarget
    {
        public abstract int TargetId { get; }
        public abstract Hex2d HexPosition { get; }
        public abstract Vector3 Position { get; }
        public abstract BaseReactiveModel BaseReactiveModel { get; }

        public abstract void Heal(float heal);

        public abstract void Hurt(float damage);

        public void UpdateAddBuff(PrioritizeLinkedList<IBuff<ITarget>> buffs, IBuff<ITarget> addedBuff)
        {
            addedBuff.ApplyBuff(this);
        }

        public void UpdateRemoveBuffs(PrioritizeLinkedList<IBuff<ITarget>> buffs, IEnumerable<IBuff<ITarget>> removedBuffs)
        {
            foreach (var removedBuff in removedBuffs)
            {
                removedBuff.ApplyBuff(this);
            }
        }
    }
}