using System;
using System.Collections.Generic;
using Match.Field.Shooting;
using Tools;
using Tools.Interfaces;
using UnityEngine.UIElements;

namespace BuffLogic
{
    public sealed class BuffManager : BaseDisposable, IOuterLogicUpdatable
    {
        private readonly Dictionary<Type, IBaseBuffManager> _buffManagers;

        public BuffManager()
        {
            _buffManagers = new Dictionary<Type, IBaseBuffManager>();
        }

        public void AddBuff<TValue>(IBuffableValue targetValue, IBuff<TValue> buff)
        {
            if (!_buffManagers.TryGetValue(typeof(TValue), out IBaseBuffManager buffManager))
            {
                buffManager = AddDisposable(new BaseBuffManager<TValue>());
                _buffManagers.Add(typeof(TValue), buffManager);
            }
            
            buffManager.AddBuff(targetValue, buff);
        }
        
        public bool HasBuffs(IBuffableValue targetValue)
        {
            if (_buffManagers.TryGetValue(typeof(IBuffableValue), out IBaseBuffManager buffManager))
            {
                return buffManager.IsBuffs(targetValue);
            }

            return false;
        }

        public IEnumerable<TValue> GetBuffsOf<TValue>(IBuffableValue<TValue> targetValue)
        {
            if (_buffManagers.TryGetValue(typeof(TValue), out IBaseBuffManager buffManager))
            {
                return buffManager.GetBuffOf(targetValue);
            }

            return new List<TValue>();
        }

        protected override void OnDispose()
        {
            _buffManagers.Clear();
        }

        public void OuterLogicUpdate(float frameLength)
        {
            foreach (var buffManager in _buffManagers)
            {
                buffManager.Value.OuterLogicUpdate(frameLength);
            }
        }
    }
}