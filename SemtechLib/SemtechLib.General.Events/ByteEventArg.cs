using System;

namespace SemtechLib.General.Events
{
	public sealed class ByteEventArg : EventArgs
	{
        public byte Value { get; }

        public ByteEventArg(byte value)
		{
			Value = value;
		}
	}
}
