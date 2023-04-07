using System;
using Newtonsoft.Json;

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
			return Q << 8 | R;
		}

		public static explicit operator Hex2d(Hex3d x) => new Hex2d(x.Q, x.R);

//		public static Hex3d ToHex3d(Hex2d hex2d, int h = 0) => new Hex3d(hex2d.Q, hex2d.R, h);

		public override string ToString() => $"Hex2d({Q},{R},{S})";
	}
}