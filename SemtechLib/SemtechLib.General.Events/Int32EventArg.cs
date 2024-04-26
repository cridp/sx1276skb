using System;

namespace SemtechLib.General.Events
{
	public sealed class Int32EventArg : EventArgs
	{
        public int Value { get; }

        public Int32EventArg(int value)
		{
			Value = value;
		}
	}
}
