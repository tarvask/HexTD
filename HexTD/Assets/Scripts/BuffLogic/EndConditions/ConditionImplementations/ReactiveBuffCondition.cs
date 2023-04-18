using System;
using Tools;
using UniRx;

namespace BuffLogic
{
    public class ReactiveBuffCondition<T> : BaseDisposable, IBuffCondition
    {
        private readonly IObservable<T> _observable;
        private bool _isConditionDone;

        public ReactiveBuffCondition(IObservable<T> observable)
        {
            _observable = observable;
            _isConditionDone = false;
            AddDisposable(_observable.Subscribe(_ => _isConditionDone = true));
        }
 
        public bool Invoke()
        {
            return _isConditionDone;
        }
    }
}