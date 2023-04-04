using System;
using System.Collections;
using System.Collections.Generic;

namespace UI.Tools
{
    public abstract class ElementListPool<T> : IDisposable, IEnumerable<T>
    {
        private readonly LinkedList<T> _activeElements;
        private readonly Queue<T> _pooledElements;

        public ElementListPool()
        {
            _activeElements = new LinkedList<T>();
            _pooledElements = new Queue<T>();
        }
        
        public T GetElement()
        {
            if (_pooledElements.TryDequeue(out T elementView))
            {
                InitElement(elementView);
            }
            else
            {
                elementView = CreateNewElement();
            }
            
            _activeElements.AddFirst(elementView);

            return elementView;
        }
        
        public void RemoveElement(T elementView)
        {
            CacheElement(elementView);
            _activeElements.Remove(elementView);
            _pooledElements.Enqueue(elementView);
        }

        protected abstract T CreateNewElement();
        protected abstract void InitElement(T element);
        protected abstract void CacheElement(T element);
        protected abstract void DisposeElement(T element);

        public void ClearList()
        {
            for(var node = _activeElements.First; node != null; )
            {
                var element = node.Value;
                node = node.Next;
                RemoveElement(element);
            }
        }
        
        public void Dispose()
        {
            foreach (var pooledElement in _pooledElements)
                DisposeElement(pooledElement);
            
            foreach (var activeElement in _activeElements)
                DisposeElement(activeElement);
            
            _pooledElements.Clear();
            _activeElements.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _activeElements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _activeElements.GetEnumerator();
        }
    }
}