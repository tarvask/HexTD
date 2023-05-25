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

        protected void ApplyVfx(IBuff buff, ITarget target, VfxManager vfxManager)
        {
            if(vfxPrefab == null)
                return;
            
            vfxManager.AddVfx(target, vfxPrefab);
            buff.SubscribeOnEnd(() => vfxManager.ReleaseVfx(target));
        }
    }
}