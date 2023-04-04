using System;

namespace HexSystem
{
	public struct Orientation
	{
		public readonly double F0;
		public readonly double F1;
		public readonly double F2;
		public readonly double F3;

		public readonly double B0;
		public readonly double B1;
		public readonly double B2;
		public readonly double B3;

		public readonly float StartAngle; //in multiples of 60
		public readonly float StartAngleDegrees => 60 * StartAngle;

		public Orientation(double f0, double f1, double f2, double f3, double b0, double b1, double b2, double b3,
			float startAngle)
		{
			F0 = f0;
			F1 = f1;
			F2 = f2;
			F3 = f3;
			B0 = b0;
			B1 = b1;
			B2 = b2;
			B3 = b3;
			StartAngle = startAngle;
		}

		public static Orientation LayoutPointy()
		{
			return new Orientation(Math.Sqrt(3.0), Math.Sqrt(3.0) / 2.0, 0.0, 3.0 / 2.0,
				Math.Sqrt(3.0) / 3.0, -1.0 / 3.0, 0.0, 2.0 / 3.0, 0.5f);
		}

		public static Orientation LayoutFlat()
		{
			return new Orientation(3.0 / 2.0, 0.0, Math.Sqrt(3.0) / 2.0, Math.Sqrt(3.0),
				2.0 / 3.0, 0.0, -1.0 / 3.0, Math.Sqrt(3.0) / 3.0, 0.0f);
		}
	}
}