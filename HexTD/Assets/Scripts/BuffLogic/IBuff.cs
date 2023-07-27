using System;
using Tools.Interfaces;
using Tools.PriorityTools;

namespace BuffLogic
{
    public interface IBuff : IDisposable, IOuterLogicUpdatable, IPrioritizatedModule, ISerializableToNetwork, ISerializableFromNetwork
    {
        bool IsEndConditionDone { get; }
        void SubscribeOnEnd(Action onEnd);
        void MergeBuffs<TBuff>(TBuff buff) where TBuff : IBuff;
    }
    
    public interface IBuff<T> : IBuff
    {
        T ApplyBuff(T value);
        T RevokeBuff(T value);
    }
}