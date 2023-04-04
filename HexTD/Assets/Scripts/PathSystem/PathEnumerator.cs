using System.Collections;
using System.Collections.Generic;
using HexSystem;

namespace PathSystem
{
    public class PathEnumerator : IEnumerator<Hex2d>
    {
        private readonly LinkedList<Hex2d> _points;
        private LinkedListNode<Hex2d> _currentNode;
        public Hex2d Current => _currentNode?.Value ?? new Hex2d();
        object IEnumerator.Current => Current;

        public PathEnumerator(LinkedList<Hex2d> points)
        {
            _points = points;
            Reset();
        }
            
        public bool MoveNext()
        {
            _currentNode = _currentNode?.Next;
            return _currentNode != null;
        }

        public void Reset()
        {
            _currentNode = _points.First;
        }

        public void Dispose()
        {
            _currentNode = null;
        }
    }
}