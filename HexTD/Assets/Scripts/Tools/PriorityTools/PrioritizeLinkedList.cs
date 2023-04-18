using System.Collections;
using System.Collections.Generic;

namespace Tools.PriorityTools
{
    public class PrioritizeLinkedList<T> : BaseDisposable, IEnumerable<T> where T : IPrioritizatedModule
    {
        protected readonly LinkedList<T> ValueList;

        public PrioritizeLinkedList()
        {
            ValueList = new LinkedList<T>();
        }

        public void Add(T value)
        {
            for (var valueNode = ValueList.First; valueNode != null; valueNode = valueNode.Next)
            {
                if (valueNode.Value.Priority == value.Priority || 
                    valueNode.Value.Priority > value.Priority)
                {
                    ValueList.AddBefore(valueNode, new LinkedListNode<T>(value));
                    return;
                }
            }

            if (ValueList.First == null)
                ValueList.AddFirst(value);
        }

        public void Remove(T value)
        {
            ValueList.Remove(value);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ValueList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected override void OnDispose()
        {
            ValueList.Clear();
        }
    }
}