using UnityEngine;

namespace HexSystem
{
	public struct Layout
	{
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
		}

		public Vector3 ToPlane(Hex3d hex)
		{
			return ToPlane(hex.Q, hex.R, hex.H);
		}

		public Vector3 ToPlane(Hex2d hex)
		{
			return ToPlane(hex.Q, hex.R);
		}

		public Vector3 ToPlane(int q, int r, int h = 0)
		{
			float x = (float)((Orientation.F0 * q + Orientation.F1 * r) * Size.x);
			float y = (float)(h * Size.y);
			float z = (float)((Orientation.F2 * q + Orientation.F3 * r) * Size.z);
			return new Vector3(x + Origin.x, y + Origin.y, z + Origin.z);
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
	}
}