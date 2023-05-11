﻿using System;
using Match.Field.Shooting;
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
            Less = 2,
            Multiply = 3,
            Divide = 4
        }
        
        [SerializeField] private InfluenceType influenceType = InfluenceType.Divide;
        [SerializeField] private EntityBuffableValueType entityBuffableValueType;
        [SerializeField] private float buffValue;
        [SerializeField] private float duration;
        
        public override void ApplyBuff(ITarget target, BuffManager buffManager)
        {
            BaseValueBuff<float> buff = GetTypedBuff();
            buff.AddCondition(new TimerBuffCondition(duration));
            
            if(target.BaseReactiveModel.TryGetBuffableValue(entityBuffableValueType,
                   out var buffableValue))
                buffManager.AddBuff(buffableValue, buff);
        }

        private BaseValueBuff<float> GetTypedBuff()
        {
            switch (influenceType)
            {
                case InfluenceType.Add:
                    return new AddFloatValueBuff(buffValue);
                case InfluenceType.Less:
                    return new AddFloatValueBuff(-buffValue);
                case InfluenceType.Multiply:
                    return new MultiFloatValueBuff(buffValue);
                case InfluenceType.Divide:
                    return new DivideFloatValueBuff(buffValue);
            }

            throw new Exception("Buff with unknown influence type!");
        }
    }
}