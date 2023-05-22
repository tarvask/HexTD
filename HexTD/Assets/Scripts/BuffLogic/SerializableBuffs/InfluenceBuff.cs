using System;
using Match.Field.Shooting;
using Match.Field.VFX;
using UnityEngine;

namespace BuffLogic.SerializableBuffs
{
    
    [Serializable]
    public class InfluenceBuff : BaseSerializableBuff
    {
        private enum InfluenceType
        {
            Undefined = 0,
        
            Add = 1,
            //Less = 2,
            Multiply = 3,
            //Divide = 4
        }
        
        [SerializeField] private InfluenceType influenceType = InfluenceType.Multiply;
        [SerializeField] private EntityBuffableValueType entityBuffableValueType;
        [SerializeField] private float buffValue;
        [SerializeField] private float duration;

        public override void ApplyBuff(ITarget target, BuffManager buffManager, VfxManager vfxManager)
        {
            BaseValueBuff<float> buff = GetTypedBuff();
            buff.AddCondition(new TimerBuffCondition(duration));
            
            if(!target.BaseReactiveModel.TryGetBuffableValue(entityBuffableValueType,
                   out var buffableValue))
                return;
            
            buffManager.AddBuff(buffableValue, buff);
            ApplyVfx(buff, target, vfxManager);
        }

        private BaseValueBuff<float> GetTypedBuff()
        {
            switch (influenceType)
            {
                case InfluenceType.Add:
                    return new AddFloatValueBuff(buffValue);
                case InfluenceType.Multiply:
                    return new MultiFloatValueBuff(buffValue);
            }

            throw new Exception("Buff with unknown influence type!");
        }
    }
}