using System;
using System.Collections.Generic;
using Tools;
using Tools.Interfaces;

namespace Match.Field.Buff
{
    public class BaseBuffsManager : BaseDisposable, IOuterLogicUpdatable
    {
        // main data
        private readonly Dictionary<int, AbstractBuffModel> _buffs;
        // supporting data
        protected readonly Dictionary<byte, AbstractBuffedParameter> _buffedParametersByTypes;
        private readonly Dictionary<int, AbstractBuffModel> _buffsToRemove;
        private int _lastBuffId;

        public Dictionary<int, AbstractBuffModel> Buffs => _buffs;

        protected BaseBuffsManager()
        {
            _buffs = new Dictionary<int, AbstractBuffModel>();
            
            Array buffTypes = Enum.GetValues(typeof(BuffedParameterType));
            _buffedParametersByTypes = new Dictionary<byte, AbstractBuffedParameter>(buffTypes.Length);
            _buffsToRemove = new Dictionary<int, AbstractBuffModel>();
        }
        
        public float ParameterResultValue(BuffedParameterType parameterType)
        {
            return _buffedParametersByTypes[(byte) parameterType].ResultBuffedValue;
        }
        
        public void OuterLogicUpdate(float frameLength)
        {
            foreach (KeyValuePair<int, AbstractBuffModel> buffKeyPair in _buffs)
            {
                buffKeyPair.Value.OuterLogicUpdate(frameLength);
                
                if (buffKeyPair.Value.HasRemainingDuration)
                    continue;

                _buffsToRemove.Add(buffKeyPair.Key, buffKeyPair.Value);
            }
            
            // remove
            if (_buffsToRemove.Count == 0)
                return;
            
            foreach (KeyValuePair<int, AbstractBuffModel> removingBuffKeyPair in _buffsToRemove)
            {
                RemoveBuff(removingBuffKeyPair.Key, removingBuffKeyPair.Value.BuffParameters.BuffedParameterType);
            }
            
            _buffsToRemove.Clear();
        }

        public void AddBuff(AbstractBuffParameters newBuffParams)
        {
            _lastBuffId++;
            _buffs.Add(_lastBuffId, new AbstractBuffModel(newBuffParams));
            // add to particular parameter type
            _buffedParametersByTypes[(byte)newBuffParams.BuffedParameterType].AddBuff(_lastBuffId, newBuffParams);
        }

        public void RemoveBuff(BuffedParameterType removingBuffPropertyTypeByte, byte removingBuffAbilitySubtypeByte)
        {
            if (_buffedParametersByTypes[(byte)removingBuffPropertyTypeByte].TryGetBuffBySubtype(removingBuffAbilitySubtypeByte,
                out int buffKey))
            {
                RemoveBuff(buffKey, removingBuffPropertyTypeByte);
            }
            // else
            // {
            //     throw new ArgumentException($"Tried to remove non-existing buff {removingBuffAbilitySubtypeByte}" +
            //                                 $" of property {removingBuffPropertyTypeByte}");
            // }
        }
        
        private void RemoveBuff(int removingBuffKey, BuffedParameterType removingBuffPropertyTypeByte)
        {
            _buffs.Remove(removingBuffKey);
            // remove from particular parameter type
            _buffedParametersByTypes[(byte)removingBuffPropertyTypeByte].RemoveBuff(removingBuffKey);
        }
        
        public bool HasBuff(AbstractBuffParameters buff)
        {
            return _buffedParametersByTypes.TryGetValue((byte) buff.BuffedParameterType, out AbstractBuffedParameter buffedParameter)
                   && buffedParameter.TryGetBuffBySubtype(buff.BuffSubType, out int buffKey);
        }
        
        public void ClearBuffs()
        {
            _buffs.Clear();

            foreach (KeyValuePair<byte, AbstractBuffedParameter> buffedParameterKeyPair in _buffedParametersByTypes)
            {
                buffedParameterKeyPair.Value.ClearBuffs();
            }
        }
    }
}