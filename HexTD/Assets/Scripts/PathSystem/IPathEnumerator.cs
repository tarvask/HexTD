using System.Collections.Generic;
using HexSystem;
using UnityEngine;

namespace PathSystem
{
    public interface IPathEnumerator : IEnumerator<Hex2d>
    {
        byte CurrentPointIndex { get; }
        float PathLength { get; }

        void MoveTo(int pointIndex);
    }
}