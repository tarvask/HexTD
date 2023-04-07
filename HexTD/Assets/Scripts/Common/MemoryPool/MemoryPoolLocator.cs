using System;
using System.Threading;
using UnityEngine;

namespace Common.MemoryPool
{
    public static class MemoryPoolLocator
    {
        public static IAsyncMemoryPool<T> LocateAsyncPool<T>(Func<IAsyncMemoryPool<T>> alloc)
            where T : Component =>
            LazyInitializer.EnsureInitialized(ref Implementation<T>.AsyncPoolInstance, alloc);

        public static IMemoryPool<T> LocatePool<T>(Func<IMemoryPool<T>> alloc)
            where T : Component =>
            LazyInitializer.EnsureInitialized(ref Implementation<T>.PoolInstance, alloc);

        private static class Implementation<T> where T : Component
        {
            public static IAsyncMemoryPool<T> AsyncPoolInstance;
            public static IMemoryPool<T> PoolInstance;
        }
    }
}