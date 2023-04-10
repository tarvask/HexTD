using System;

namespace HexSystem
{
	public readonly struct FractionalHex
	{
		public readonly double Q;
		public readonly double R;
		public readonly double S;

		public readonly double H;

		public FractionalHex(double q, double r, double h)
		{
			Q = q;
			R = r;
			S = -q - r;
			H = h;
		}

		public FractionalHex(double q, double r, double s, double h)
		{
			Q = q;
			R = r;
			S = s;
			H = h;
		}

		public FractionalHex(Hex2d hex)
		{
			Q = hex.Q;
			R = hex.R;
			S = hex.S;
			H = 0;
		}

		public FractionalHex(Hex3d hex)
		{
			Q = hex.Q;
			R = hex.R;
			S = hex.S;
			H = hex.H;
		}

		public Hex3d RoundToHex()
		{
			var roundQ = (int)Math.Round(Q);
			var roundR = (int)Math.Round(R);
			var roundS = (int)Math.Round(S);
			var roundH = (int)Math.Round(H);

			var qDiff = Math.Abs(Q - roundQ);
			var rDiff = Math.Abs(R - roundR);
			var sDiff = Math.Abs(S - roundS);

			if (qDiff > rDiff && qDiff > sDiff)
				roundQ = -roundR - roundS;
			else if (rDiff > sDiff)
				roundR = -roundQ - roundS;
			else
				roundS = -roundQ - roundR;

			return new Hex3d(roundQ, roundR, roundS, roundH);
		}

		public static FractionalHex operator *(FractionalHex a, double f)
		{
			return new FractionalHex(a.Q * f, a.R * f, a.S * f, a.H * f);
		}

		public static FractionalHex operator /(FractionalHex a, double f)
		{
			return new FractionalHex(a.Q / f, a.R / f, a.S / f, a.H / f);
		}

		public override string ToString() => $"FractionalHex({Q},{R},{S})";
	}
}