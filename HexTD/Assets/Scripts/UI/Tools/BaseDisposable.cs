using System;
using System.Collections.Generic;

namespace UI.Tools
{
    public class BaseDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables;

        public BaseDisposable()
        {
            _disposables = new List<IDisposable>();
        }

        protected T AddDisposable<T>(T disposable) where T : IDisposable
        {
            _disposables.Add(disposable);
            return disposable;
        }
        
        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}