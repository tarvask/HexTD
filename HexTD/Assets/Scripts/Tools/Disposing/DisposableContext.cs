using System;
using UniRx;

namespace Tools.Disposing
{
    public class DisposableContext : IDisposableContext
    {
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public void Dispose() => _compositeDisposable.Dispose();
        public void AddOwnership(IDisposable disposable) => _compositeDisposable.Add(disposable);
        public void RemoveOwnership(IDisposable disposable) => _compositeDisposable.Remove(disposable);
    }
}