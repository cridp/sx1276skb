using System;

namespace SemtechLib.General.Events
{
	public sealed class DecimalEventArg : EventArgs
	{
        public decimal Value { get; }

        public DecimalEventArg(decimal value)
		{
			Value = value;
		}
	}
}
