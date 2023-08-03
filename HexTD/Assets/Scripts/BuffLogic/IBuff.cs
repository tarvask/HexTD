using System;
using Match.Serialization;
using Tools.Interfaces;
using Tools.PriorityTools;

namespace BuffLogic
{
    public interface IBuff : IDisposable, IOuterLogicUpdatable, IPrioritizatedModule, ISerializableToNetwork
    {
        Type TargetType { get; }
        bool IsEndConditionDone { get; }
        void SubscribeOnEnd(Action onEnd);
        void MergeBuffs<TBuff>(TBuff buff) where TBuff : IBuff;
    }
    
    public interface IBuff<T> : IBuff
    {
        Type IBuff.TargetType => typeof(T);
        
        T ApplyBuff(T value);
        T RevokeBuff(T value);
    }
}