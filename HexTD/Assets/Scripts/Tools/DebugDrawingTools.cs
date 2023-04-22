using UnityEngine;

namespace Tools
{
	public class DebugDrawingTools
	{
		public static void DrawBounds(Bounds bounds)
		{
			float duration = 0.0f;
			Color white = Color.white;
			DrawBounds(bounds, white, duration);
		}
		
		public static void DrawBounds(Bounds bounds, Color color, float duration)
		{
			var pointLeftBottomFront = bounds.min;
			var pointLeftTopFront = pointLeftBottomFront + Vector3.up * bounds.size.y;
			var pointLeftTopBack = pointLeftTopFront + Vector3.forward * bounds.size.z;
			var pointLeftBottomBack = pointLeftTopBack + Vector3.down * bounds.size.y;
			var pointRightTopBack = bounds.max;
			var pointRightBottomBack = pointRightTopBack + Vector3.down * bounds.size.y;
			var pointRightBottomFront = pointRightBottomBack + Vector3.back * bounds.size.z;
			var pointRightTopFront = pointRightBottomFront + Vector3.up * bounds.size.y;

			Debug.DrawLine(pointLeftBottomFront, pointLeftTopFront, color, duration);
			Debug.DrawLine(pointLeftTopFront, pointLeftTopBack, color, duration);
			Debug.DrawLine(pointLeftTopBack, pointLeftBottomBack, color, duration);
			Debug.DrawLine(pointLeftBottomBack, pointLeftBottomFront, color, duration);
			Debug.DrawLine(pointRightBottomFront, pointRightTopFront, color, duration);
			Debug.DrawLine(pointRightTopFront, pointRightTopBack, color, duration);
			Debug.DrawLine(pointRightTopBack, pointRightBottomBack, color, duration);
			Debug.DrawLine(pointRightBottomBack, pointRightBottomFront, color, duration);
			Debug.DrawLine(pointLeftBottomFront, pointRightBottomFront, color, duration);
			Debug.DrawLine(pointLeftTopFront, pointRightTopFront, color, duration);
			Debug.DrawLine(pointLeftTopBack, pointRightTopBack, color, duration);
			Debug.DrawLine(pointLeftBottomBack, pointRightBottomBack, color, duration);
		}
	}
}