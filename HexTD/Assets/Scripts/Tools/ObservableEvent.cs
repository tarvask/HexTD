using System;
using System.Collections.Generic;

namespace Tools
{
    public interface ISubscribeOnlyObservableEvent : IDisposable
    {
        void Subscribe(Action method);
        void Unsubscribe(Action method);
    }

    public interface ISubscribeOnlyObservableEvent<out T> : IDisposable
    {
        void Subscribe(Action<T> method);
        void Unsubscribe(Action<T> method);
    }

    public interface ISubscribeOnlyObservableEvent<out T1, out T2> : IDisposable
    {
        void Subscribe(Action<T1, T2> method);
        void Unsubscribe(Action<T1, T2> method);
    }
    
    public interface ISubscribeOnlyObservableEvent<out T1, out T2, out T3> : IDisposable
    {
        void Subscribe(Action<T1, T2, T3> method);
        void Unsubscribe(Action<T1, T2, T3> method);
    }

    public interface IObservableEvent : ISubscribeOnlyObservableEvent
    {
        void Fire();
    }

    public interface IObservableEvent<T> : ISubscribeOnlyObservableEvent<T>
    {
        void Fire(T eventParameter);
    }

    public interface IObservableEvent<T1, T2> : ISubscribeOnlyObservableEvent<T1, T2>
    {
        void Fire(T1 eventParameter1, T2 eventParameter2);
    }
    
    public interface IObservableEvent<T1, T2, T3> : ISubscribeOnlyObservableEvent<T1, T2, T3>
    {
        void Fire(T1 eventParameter1, T2 eventParameter2, T3 eventParameter3);
    }

    public class ObservableEvent : IObservableEvent
    {
        private readonly List<Action> _subscribers = new List<Action>();
        
        public void Fire()
        {
            foreach (Action method in _subscribers)
            {
                method.Invoke();
            }
        }

        public void Subscribe(Action method)
        {
            _subscribers.Add(method);
        }
        
        public void Unsubscribe(Action method)
        {
            _subscribers.Remove(method);
        }

        public void Dispose()
        {
            _subscribers.Clear();
        }
    }
    
    public class ObservableEvent<T> : IObservableEvent<T>
    {
        private readonly List<Action<T>> _subscribers = new List<Action<T>>();
        
        public void Fire(T eventParameter)
        {
            foreach (Action<T> method in _subscribers)
            {
                method.Invoke(eventParameter);
            }
        }

        public void Subscribe(Action<T> method)
        {
            _subscribers.Add(method);
        }
        
        public void Unsubscribe(Action<T> method)
        {
            _subscribers.Remove(method);
        }
        
        public void Dispose()
        {
            _subscribers.Clear();
        }
    }
    
    public class ObservableEvent<T1, T2> : IObservableEvent<T1, T2>
    {
        private readonly List<Action<T1,T2>> _subscribers = new List<Action<T1,T2>>();
        
        public void Fire(T1 eventParameter1, T2 eventParameter2)
        {
            foreach (Action<T1,T2> method in _subscribers)
            {
                method.Invoke(eventParameter1, eventParameter2);
            }
        }

        public void Subscribe(Action<T1, T2> method)
        {
            _subscribers.Add(method);
        }

        public void Unsubscribe(Action<T1, T2> method)
        {
            _subscribers.Remove(method);
        }
        
        public void Dispose()
        {
            _subscribers.Clear();
        }
    }
    
    public class ObservableEvent<T1, T2, T3> : IObservableEvent<T1, T2, T3>
    {
        private readonly List<Action<T1, T2, T3>> _subscribers = new List<Action<T1, T2, T3>>();
        
        public void Fire(T1 eventParameter1, T2 eventParameter2, T3 eventParameter3)
        {
            foreach (Action<T1, T2, T3> method in _subscribers)
            {
                method.Invoke(eventParameter1, eventParameter2, eventParameter3);
            }
        }

        public void Subscribe(Action<T1, T2, T3> method)
        {
            _subscribers.Add(method);
        }

        public void Unsubscribe(Action<T1, T2, T3> method)
        {
            _subscribers.Remove(method);
        }
        
        public void Dispose()
        {
            _subscribers.Clear();
        }
    }
}