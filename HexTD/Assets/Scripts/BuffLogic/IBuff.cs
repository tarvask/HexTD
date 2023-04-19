using Tools.PriorityTools;

namespace BuffLogic
{
    public interface IBuff { }
    
    public interface IBuff<T> : IPrioritizatedModule, IBuff
    {
        bool IsEndConditionDone { get; }
        T ApplyBuff(T value);
        T RevokeBuff(T value);
        void Update();
        void MergeBuffs<TBuff>(TBuff buff) where TBuff : IBuff<T>;
    }
}