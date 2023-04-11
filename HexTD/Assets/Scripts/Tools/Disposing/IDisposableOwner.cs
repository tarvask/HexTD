using System;

namespace Tools.Disposing
{
    public interface IDisposableOwner
    {
        void AddOwnership(IDisposable disposable);
        void RemoveOwnership(IDisposable disposable);
    }

    public static class DisposableOwnerExtensions
    {
        public static T AddTo<T>(this T disposable, IDisposableOwner disposableOwner) 
            where T : IDisposable
        {
            if (disposable == null) throw new ArgumentNullException(nameof(disposable));
            if (disposableOwner == null) throw new ArgumentNullException(nameof(disposableOwner));
            
            disposableOwner.AddOwnership(disposable);
            return disposable;
        }
    }
}