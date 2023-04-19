using System.Collections.Generic;
using Tools;
using Tools.Interfaces;

namespace BuffLogic
{
    public interface IBaseBuffManager : IOuterLogicUpdatable
    {
        void AddBuff<T>(IBuffableValue targetValue, IBuff<T> buff);
        IEnumerable<T> GetBuffOf<T>(IBuffableValue<T> targetValue);
    }
    
    public class BaseBuffManager<TValue> : BaseDisposable, IBaseBuffManager
    {
        private readonly Dictionary<IBuffableValue<TValue>, PrioritizedBuffLinkedList<TValue>> _buffs;

        public BaseBuffManager()
        {
            _buffs = new Dictionary<IBuffableValue<TValue>, PrioritizedBuffLinkedList<TValue>>(5);
        }

        public void AddBuff(IBuffableValue<TValue> targetValue, IBuff<TValue> buff)
        {
            if (!_buffs.TryGetValue(targetValue, out var buffList))
            {
                buffList = new PrioritizedBuffLinkedList<TValue>(targetValue);
                _buffs.Add(targetValue, buffList);
            }
            
            buffList.AddBuff(buff);
        }

        public void AddBuff<T>(IBuffableValue targetValue, IBuff<T> buff)
        {
            AddBuff((IBuffableValue<TValue>)targetValue, (IBuff<TValue>)buff);
        }

        public IEnumerable<T> GetBuffOf<T>(IBuffableValue<T> targetValue)
        {
            var target = (IBuffableValue<TValue>)targetValue;

            if (!_buffs.TryGetValue(target, out var buffList))
                return new List<T>();

            return (IEnumerable<T>) buffList;
        }

        public void OuterLogicUpdate(float frameLength)
        {
            foreach (var buffValuePair in _buffs)
            {
                buffValuePair.Value.OuterLogicUpdate(frameLength);
            }
        }
    }
}