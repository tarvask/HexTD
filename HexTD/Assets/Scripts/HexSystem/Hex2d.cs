using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace HexSystem
{
	public readonly struct Hex2d : IEquatable<Hex2d>
	{
		[JsonProperty("Q")]public readonly int Q;
		[JsonProperty("R")]public readonly int R;
		[JsonIgnore]public readonly int S;

		public Hex2d(int q, int r)
		{
			Q = q;
			R = r;
			S = -q - r;
		}

		public Hex2d(int q, int r, int s)
		{
			Q = q;
			R = r;
			S = s;
		}
		
		public static readonly Hex2d[] NearOffsets =
		{
			new Hex2d(1, 0),
			new Hex2d(1, -1),
			new Hex2d(0, -1),
			new Hex2d(-1, 0),
			new Hex2d(-1, 1),
			new Hex2d(0, 1)
		};

		public static IEnumerable<Hex2d> IterateNearOffsets => NearOffsets;

		public bool Equals(Hex2d other) => Q == other.Q && R == other.R;

		public override bool Equals(object obj)
		{
			return obj is Hex2d other && Equals(other);
		}

		public static bool operator ==(Hex2d h1, Hex2d h2)
		{
			return h1.Equals(h2);
		}

		public static bool operator !=(Hex2d h1, Hex2d h2)
		{
			return !h1.Equals(h2);
		}

		public override int GetHashCode()
		{
			byte sign = Q > 0 ? (byte)1 : (byte)2;
			return (sign << 24) + (Q << 16) + R;
		}

		public static explicit operator Hex2d(Hex3d x) => new Hex2d(x.Q, x.R);

		public override string ToString() => $"Hex2d({Q},{R},{S})";

		public float DistanceTo(Hex2d other)
		{
			return (this - other).Magnitude;
		}

		public float Magnitude => (Mathf.Abs(R) + Mathf.Abs(Q) + Mathf.Abs(S)) * .5f;
		public int Length => (int)Magnitude;

		public static Hex2d RoundToHex(float q, float r, float s, float h)
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

			return new Hex2d(roundQ, roundR, roundS);
		}

		public static Hex2d operator +(Hex2d a, Hex2d b)
		{
			return new Hex2d(a.Q + b.Q, a.R + b.R);
		}

		public static Hex2d operator -(Hex2d a, Hex2d b)
		{
			return new Hex2d(a.Q - b.Q, a.R - b.R);
		}

		public static FractionalHex operator *(Hex2d a, float f)
		{
			return new FractionalHex(a) * f;
		}

		public static Hex2d operator *(Hex2d a, int value)
		{
			return new Hex2d(a.Q * value, a.R * value, a.S * value);
		}

		public static Hex2d RotateClockwise(Hex2d hex)
		{
			return new Hex2d(-hex.S, -hex.Q, -hex.R);
		}

		public static Hex2d Rotate180(Hex2d hex)
		{
			return RotateClockwise(RotateClockwise(RotateClockwise(hex)));
		}

		public static Hex2d InvertVertical(Hex2d hex)
		{
			return new Hex2d(hex.S, hex.R, hex.Q);
		}

		public static Hex2d InvertHorizontal(Hex2d hex)
		{
			return new Hex2d(-hex.S, -hex.R, -hex.R);
		}

		public static Hex2d RotateСounterClockwise(Hex2d hex)
		{
			return new Hex2d(-hex.R, -hex.S, -hex.Q);
		}

		public static IEnumerable<Hex2d> IterateLine(Hex2d from, Hex2d to)
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
				yield return from + (Hex2d)(step * i).RoundToHex();
			}
		}

		public IEnumerable<Hex2d> IterateNear()
		{
			foreach (var offsetCoord in IterateNearOffsets)
			{
				yield return this + offsetCoord;
			}
		}

		public static IEnumerable<Hex2d> IterateRing(Hex2d center, int radius)
		{
			Hex2d hex = center + NearOffsets[4] * radius;

			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < radius; j++)
				{
					yield return hex;
					hex = hex + NearOffsets[i];
				}
			}
		}

		public static IEnumerable<Hex2d> IterateSpiralRing(Hex2d center, int radius)
		{
			yield return center;

			for (int r = 1; r <= radius; r++)
			{
				foreach (Hex2d hex in IterateRing(center, r))
				{
					yield return hex;
				}
			}
		}
	}
}