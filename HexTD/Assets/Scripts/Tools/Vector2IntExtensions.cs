using UnityEngine;

namespace Tools
{
    public static class Vector2IntExtension
    {
        // default Vector2Int.GetHashCode() is unsuitable,
        // so we modify it as a throughout index as if cells were a 1-dim array
        // so fieldWidth is needed here
        public static int GetHashCode(this Vector2Int position, int fieldWidth)
        {
            return position.x + position.y * fieldWidth;
        }
    }
}