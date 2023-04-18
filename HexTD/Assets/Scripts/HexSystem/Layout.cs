using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HexSystem
{
	public struct Layout
	{
		/// <summary>
		/// One sixth of a full rotation (radians).
		/// </summary>
		private static readonly float SEXTANT = Mathf.PI / 3;

		// Corner locations in XY space. Private for same reason as neighbors.
		private readonly Vector2[] corners;
		
		public readonly Vector3 Size;
		public readonly Vector3 Origin;	
		public readonly bool IsOrientationFlat;
		public readonly Orientation Orientation;

		public Layout(Vector3 size, Vector3 origin, bool isOrientationFlat)
		{
			IsOrientationFlat = isOrientationFlat;
			Orientation = IsOrientationFlat ? Orientation.LayoutFlat() : Orientation.LayoutPointy();
			Size = size;
			Origin = origin;

			//todo now only for non-flat orientation
			Vector2 to2dSize = new Vector2(Size.x, Size.z);
			corners = new[]
			{
				to2dSize * new Vector2(Mathf.Sin(SEXTANT), Mathf.Cos(SEXTANT)),
				to2dSize * new Vector2(0, 1),
				to2dSize * new Vector2(Mathf.Sin(-SEXTANT), Mathf.Cos(-SEXTANT)),
				to2dSize * new Vector2(Mathf.Sin(Mathf.PI + SEXTANT), Mathf.Cos(Mathf.PI - SEXTANT)),
				to2dSize * new Vector2(0, -1),
				to2dSize * new Vector2(Mathf.Sin(Mathf.PI - SEXTANT), Mathf.Cos(Mathf.PI - SEXTANT))
			};
		}

		public Vector3 ToPlane(Hex3d hex, bool isWorld = true)
		{
			return ToPlane(hex.Q, hex.R, hex.H, isWorld);
		}

		public Vector3 ToPlane(Hex2d hex, bool isWorld = true)
		{
			return ToPlane(hex.Q, hex.R, 0, isWorld);
		}

		public Vector3 ToPlane(int q, int r, int h, bool isWorld = true)
		{
			Vector3 offset = isWorld ? Origin : Vector3.zero;
			float x = (float)((Orientation.F0 * q + Orientation.F1 * r) * Size.x);
			float y = (float)(h * Size.y);
			float z = (float)((Orientation.F2 * q + Orientation.F3 * r) * Size.z);
			return new Vector3(x + offset.x, y + offset.y, z + offset.z);
		}

		public FractionalHex ToHex(Vector3 positionInPlane)
		{
			Orientation orientation = Orientation;
			Vector3 pt = new Vector3((positionInPlane.x - Origin.x) / Size.x, (positionInPlane.y - Origin.y) / Size.y,
				(positionInPlane.z - Origin.z) / Size.z);
			double q = orientation.B0 * pt.x + orientation.B1 * pt.z;
			double r = orientation.B2 * pt.x + orientation.B3 * pt.z;
			return new FractionalHex(q, r, -q - r);
		}

		public Hex3d OffsetToHex(Vector3Int offsetCoord)
		{
			return OffsetToHex(offsetCoord.x, offsetCoord.z, offsetCoord.y);
		}

		public Hex3d OffsetToHex(int col, int row, int high)
		{
			if (IsOrientationFlat)
			{
				return new Hex3d(
					col,
					(int)(row - (col - (col & 1)) * .5f),
					high);
			}
			else
			{
				return new Hex3d(
					(int)(col - (row - (row & 1)) * .5f),
					row,
					high);
			}
		}

		public Vector3Int HexToOffset(Hex3d hex)
		{
			if (IsOrientationFlat)
			{
				return new Vector3Int(
					hex.Q,
					(int)(hex.R + (hex.Q - (hex.Q & 1)) * .5f),
					hex.H);
			}
			else
			{
				return new Vector3Int(
					(int)(hex.Q + (hex.R - (hex.R & 1)) * .5f),
					hex.R,
					hex.H);
			}
		}

		/// <summary>
		/// Get the Unity position of a corner vertex.
		/// </summary>
		/// <remarks>
		/// Corner 0 is at the upper right, others proceed counterclockwise.
		/// </remarks>
		/// <param name="index">Index of the desired corner. Cyclically constrained 0..5.</param>
		public Vector2 Corner(Hex2d hex, int index)
		{
			var toplane = ToPlane(hex);
			return corners[index] + new Vector2(toplane.x, toplane.z);
//			return corners[index] + GetHexPosition(hex);
		}

		/// <summary>
		/// Determines whether this the is on the line segment between points a and b.
		/// </summary>
		public bool IsOnSegment(Hex2d hex, Vector2 a, Vector2 b)
		{
			Vector2 ab = b - a;


//			Vector2 rand = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
//			var randcol = Random.ColorHSV();
////			Vector3 aa = new Vector3((a + rand).x, 0.0f, (a + rand).y);
////			Vector3 bb = new Vector3((b + rand).x, 0.0f, (b + rand).y);
//			Vector3 aabb = new Vector3((ab + rand).x, 0.0f, (ab + rand).y);
////			Debug.DrawLine(aa,bb,randcol,5.0f);
//			Debug.DrawLine(Vector3.zero,aabb,randcol,5.0f);


			float abSqrMagnitude = ab.sqrMagnitude;
			Vector2 ac = Corner(hex, 0) - a;


//			Vector3 acc = new Vector3((ac + rand).x, 0.0f, (ac + rand).y);
//			Debug.DrawLine(Vector3.zero, acc,randcol,5.0f);


			bool within = ac.sqrMagnitude <= abSqrMagnitude && Vector2.Dot(ab, ac) >= 0;
			int sign = Math.Sign(Vector3.Cross(ab, ac).z);
			for (int i = 1; i < 6; i++)
			{
				ac = Corner(hex, i) - a;


//				acc = new Vector3((ac + rand).x, 0.0f, (ac + rand).y);
//				Debug.DrawLine(Vector3.zero, acc,randcol,5.0f);


				bool newWithin = ac.sqrMagnitude <= abSqrMagnitude && Vector2.Dot(ab, ac) >= 0;
				int newSign = Math.Sign(Vector3.Cross(ab, ac).z);
				if ((within || newWithin) && (sign * newSign <= 0))
					return true;
				within = newWithin;
				sign = newSign;
			}

			return false;
		}
	}
}