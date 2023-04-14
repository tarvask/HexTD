using System;
using System.Collections.Generic;
using Tools;
using Tools.Interfaces;

namespace BuffLogic
{
    public sealed class BuffManager : BaseDisposable, IOuterLogicUpdatable
    {
        private readonly Dictionary<Type, IBaseBuffManager> _buffManagers;

        public BuffManager()
        {
            _buffManagers = new Dictionary<Type, IBaseBuffManager>();
        }

        public void AddBuff<TValue>(IBuffableValue<TValue> targetValue, IBuff<TValue> buff)
        {
            if (!_buffManagers.TryGetValue(typeof(TValue), out IBaseBuffManager buffManager))
            {
                buffManager = AddDisposable(new BaseBuffManager<TValue>());
                _buffManagers.Add(typeof(TValue), buffManager);
            }
            
            buffManager.AddBuff(targetValue, buff);
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