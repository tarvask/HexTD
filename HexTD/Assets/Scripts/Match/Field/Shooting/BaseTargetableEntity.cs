using System.Collections.Generic;
using BuffLogic;
using Tools;
using Tools.PriorityTools;
using UnityEngine;

namespace Match.Field.Shooting
{
    public abstract class BaseTargetableEntity : BaseDisposable, ITargetable
    {
        public abstract int TargetId { get; }
        public abstract Vector3 Position { get; }
        public abstract BaseReactiveModel BaseReactiveModel { get; }

        public abstract void Hurt(float damage);

        public void UpdateAddBuff(PrioritizeLinkedList<IBuff<ITargetable>> buffs, IBuff<ITargetable> addedBuff)
        {
            addedBuff.ApplyBuff(this);
        }

        public void UpdateRemoveBuffs(PrioritizeLinkedList<IBuff<ITargetable>> buffs, IEnumerable<IBuff<ITargetable>> removedBuffs)
        {
            foreach (var removedBuff in removedBuffs)
            {
                removedBuff.ApplyBuff(this);
            }
        }
    }
}