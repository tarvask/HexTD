using System.Collections.Generic;
using HexSystem;
using UnityEngine;

namespace PathSystem
{
    public interface IPathEnumerator : IEnumerator<Hex2d>
    {
        byte CurrentPointIndex { get; }

        LinkedList<Hex2d> Points { get; }

        void MoveTo(int pointIndex);
    }
}