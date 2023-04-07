using Cysharp.Threading.Tasks;

namespace Common.MemoryPool
{
    public interface IAsyncMemoryPool<T> where T : class
    {
        UniTask<T> GetAsync();
        void Return(T instance);
    }
}