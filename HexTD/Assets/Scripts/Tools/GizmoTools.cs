using UnityEngine;

namespace Tools
{
    public static class GizmosTools
    {
        public static void DrawRect(Vector2 min, Vector2 max)
        {
            Vector2 lb = new Vector2(min.x, min.y);
            Vector2 lt = new Vector2(min.x, max.y);
            Vector2 rt = new Vector2(max.x, max.y);
            Vector2 rb = new Vector2(max.x, min.y);
            Gizmos.DrawLine(lb, lt);
            Gizmos.DrawLine(lt, rt);
            Gizmos.DrawLine(rt, rb);
            Gizmos.DrawLine(rb, lb);
        }

        public static void DrawRect(Rect rect)
        {
            Vector2 lb = rect.min;
            Vector2 rt = rect.max;
            Vector2 lt = new Vector2(lb.x, rt.y);
            Vector2 rb = new Vector2(rt.x, lb.y);
            Gizmos.DrawLine(lb, lt);
            Gizmos.DrawLine(lt, rt);
            Gizmos.DrawLine(rt, rb);
            Gizmos.DrawLine(rb, lb);
        }
    }
}