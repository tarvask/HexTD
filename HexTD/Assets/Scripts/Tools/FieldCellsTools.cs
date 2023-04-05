using UnityEngine;

namespace Tools
{
    public static class FieldCellsTools
    {
        public static Vector2Int GetCellForFieldPosition(Vector2 fieldPosition)
        {
            return new Vector2Int(Mathf.FloorToInt(fieldPosition.x), Mathf.FloorToInt(fieldPosition.y));
        }
    }
}