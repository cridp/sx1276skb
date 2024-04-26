namespace SemtechLib.Devices.SX1276.General
{
	public sealed class Latency(double delay, double precision)
	{
		private double Delay { get; set; } = delay;

        private double Precision { get; set; } = precision;

        public override string ToString()
		{
			return Delay + "," + Precision;
		}

		public static Latency operator +(Latency a, Latency b)
		{
			return new Latency(a.Delay + b.Delay, a.Precision + b.Precision);
		}

		public static Latency operator -(Latency a, Latency b)
		{
			return new Latency(a.Delay - b.Delay, a.Precision - b.Precision);
		}
	}
}
