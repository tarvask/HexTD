namespace Common.MemoryPool
{
    public interface IMemoryPool<T> where T : class
    {
        T Get();
        void Return(T instance);
    }
}