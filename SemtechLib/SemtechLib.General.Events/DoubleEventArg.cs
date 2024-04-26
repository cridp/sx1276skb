using System;

namespace SemtechLib.General.Events
{
	public sealed class DoubleEventArg : EventArgs
	{
        public double Value { get; }

        public DoubleEventArg(double value)
		{
			Value = value;
		}
	}
}
