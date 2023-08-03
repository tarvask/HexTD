using System;
using Match.Field.Shooting;
using Match.Field.VFX;
using UnityEngine;

namespace BuffLogic.SerializableBuffs
{
    [Serializable]
    public abstract class BaseSerializableBuff
    {
        [SerializeField] private Transform vfxPrefab;
        
        public abstract void ApplyBuff(ITarget target, BuffManager buffManager, VfxManager vfxManager);

        protected void ApplyVfx(IBuff buff, ITarget target, BuffManager buffManager, VfxManager vfxManager)
        {
            if(vfxPrefab == null)
                return;
            
            vfxManager.AddVfx(target, vfxPrefab);
            buff.SubscribeOnEnd(() => ReleaseVfx(vfxManager, buffManager, target));
        }

        private void ReleaseVfx(VfxManager vfxManager, BuffManager buffManager, ITarget target)
        {
            //if(buffManager.IsBuffs(target))
            //    return;

            foreach (EntityBuffableValueType entityBuffableValueType in Enum.GetValues(typeof(EntityBuffableValueType)))
            {
                if(target.BaseReactiveModel.TryGetBuffableValue(entityBuffableValueType, out IBuffableValue buffableValue) &&
                   buffManager.HasBuffs(buffableValue))
                    return;
            }
            
            vfxManager.ReleaseVfx(target);
        }
    }
}