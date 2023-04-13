using System;
using System.Collections.Generic;
using UnityEngine;

namespace HexSystem
{
	public readonly struct Hex3d : IEquatable<Hex3d>
	{
		public readonly int Q;
		public readonly int R;
		public readonly int S;

		public readonly int H;

		public static readonly Hex3d[] NearOffsets =
		{
			new Hex3d(1, 0, 0),
			new Hex3d(1, -1, 0),
			new Hex3d(0, -1, 0),
			new Hex3d(-1, 0, 0),
			new Hex3d(-1, 1, 0),
			new Hex3d(0, 1, 0)
		};

		public static IEnumerable<Hex3d> IterateNearOffsets => NearOffsets;

		public Hex3d(int q, int r, int h)
		{
			Q = q;
			R = r;
			S = -q - r;

			H = h;
		}

		public Hex3d(int q, int r, int s, int h)
		{
			Q = q;
			R = r;
			S = s;

			H = h;
		}
		
		public Hex3d(Hex2d hex2d, int h=0)
		{
			Q = hex2d.Q;
			R = hex2d.R;
			S = hex2d.S;

			H = h;
		}

		public bool Equals(Hex3d other) => Q == other.Q && R == other.R && H == other.H;

		public override bool Equals(object obj)
		{
			return obj is Hex3d other && Equals(other);
		}

		public static bool operator ==(Hex3d h1, Hex3d h2)
		{
			return h1.Equals(h2);
		}

		public static bool operator !=(Hex3d h1, Hex3d h2)
		{
			return !h1.Equals(h2);
		}

		public override int GetHashCode()
		{
			byte sign = Q > 0 ? (byte)1 : (byte)2;
			return (sign << 24) + (Q << 16) + R;
		}

		public override string ToString() => $"Hex({Q},{R},{S},{H})";

		public float DistanceTo(Hex3d other)
		{
			return (this - other).Magnitude;
		}

		public float Magnitude => (Mathf.Abs(R) + Mathf.Abs(Q) + Mathf.Abs(S)) * .5f;
		public int Length => (int)Magnitude;

		public static Hex3d RoundToHex(float q, float r, float s, float h)
		{
			var roundQ = Mathf.RoundToInt(q);
			var roundR = Mathf.RoundToInt(r);
			var roundS = Mathf.RoundToInt(s);
			var roundH = Mathf.RoundToInt(h);

			var qDiff = Mathf.Abs(q - roundQ);
			var rDiff = Mathf.Abs(r - roundR);
			var sDiff = Mathf.Abs(s - roundS);

			if (qDiff > rDiff && qDiff > sDiff)
				roundQ = -roundR - roundS;
			else if (rDiff > sDiff)
				roundR = -roundQ - roundS;
			else
				roundS = -roundQ - roundR;

			return new Hex3d(roundQ, roundR, roundS, roundH);
		}

		public static Hex3d operator +(Hex3d a, Hex3d b)
		{
			return new Hex3d(a.Q + b.Q, a.R + b.R, a.H + b.H);
		}

		public static Hex3d operator -(Hex3d a, Hex3d b)
		{
			return new Hex3d(a.Q - b.Q, a.R - b.R, a.H - b.H);
		}

		public static FractionalHex operator *(Hex3d a, float f)
		{
			return new FractionalHex(a) * f;
		}

		public static Hex3d operator *(Hex3d a, int value)
		{
			return new Hex3d(a.Q * value, a.R * value, a.S * value, a.H * value);
		}

		public static Hex3d RotateClockwise(Hex3d hex)
		{
			return new Hex3d(-hex.S, -hex.Q, -hex.R, hex.H);
		}

		public static Hex3d Rotate180(Hex3d hex)
		{
			return RotateClockwise(RotateClockwise(RotateClockwise(hex)));
		}

		public static Hex3d InvertVertical(Hex3d hex)
		{
			return new Hex3d(hex.S, hex.R, hex.Q, hex.H);
		}

		public static Hex3d InvertHorizontal(Hex3d hex)
		{
			return new Hex3d(-hex.S, -hex.R, -hex.R, hex.H);
		}

		public static Hex3d RotateСounterClockwise(Hex3d hex)
		{
			return new Hex3d(-hex.R, -hex.S, -hex.Q, hex.H);
		}

		public static IEnumerable<Hex3d> IterateLine(Hex3d from, Hex3d to)
		{
			int distance = ((int)from.DistanceTo(to));
			if (distance == 0)
			{
				yield return from;
				yield break;
			}

			FractionalHex step = new FractionalHex(to - from) / distance;
			for (var i = 0; i <= distance; i++)
			{
				yield return from + (step * i).RoundToHex();
			}
		}

		public IEnumerable<Hex3d> IterateNear()
		{
			foreach (var offsetCoord in IterateNearOffsets)
			{
				yield return this + offsetCoord;
			}
		}

		public static IEnumerable<Hex3d> IterateRing(Hex3d center, int radius)
		{
			Hex3d hex = center + NearOffsets[4] * radius;

			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < radius; j++)
				{
					yield return hex;
					hex = hex + NearOffsets[i];
				}
			}
		}

		public static IEnumerable<Hex3d> IterateSpiralRing(Hex3d center, int radius)
		{
			yield return center;

			for (int r = 1; r <= radius; r++)
			{
				foreach (Hex3d hex in IterateRing(center, r))
				{
					yield return hex;
				}
			}
		}
	}
}