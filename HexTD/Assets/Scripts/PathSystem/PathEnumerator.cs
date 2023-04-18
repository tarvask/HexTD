using System.Collections;
using System.Collections.Generic;
using HexSystem;
using Match.Field.Hexagons;
using UnityEngine;

namespace PathSystem
{
    public class PathEnumerator : IPathEnumerator
    {
        private readonly LinkedList<Hex2d> _points;

        private byte _currentPointIndex;
        private LinkedListNode<Hex2d> _currentNode;
        
        public Hex2d Current => _currentNode?.Value ?? new Hex2d();
        object IEnumerator.Current => Current;
        public byte CurrentPointIndex => _currentPointIndex;
        public LinkedList<Hex2d> Points => _points;

        public PathEnumerator(LinkedList<Hex2d> points)
        {
            _points = points;
            
            Reset();
        }
            
        public bool MoveNext()
        {
            _currentNode = _currentNode?.Next;
            _currentPointIndex++;
            return _currentNode != null;
        }
        
        public void MoveTo(int pointIndex)
        {
            _currentNode = _points.First;
            for (_currentPointIndex = 0; _currentPointIndex < pointIndex; _currentPointIndex++)
                _currentNode = _currentNode?.Next;
        }

        public void Reset()
        {
            _currentPointIndex = 0;
            _currentNode = _points.First;
        }

        public void Dispose()
        {
            _currentNode = null;
        }
    }
}