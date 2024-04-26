using System;

namespace SemtechLib.General.Events
{
	public sealed class Int16EventArg : EventArgs
	{
        public short Value { get; }

        public Int16EventArg(short value)
		{
			Value = value;
		}
	}
}
