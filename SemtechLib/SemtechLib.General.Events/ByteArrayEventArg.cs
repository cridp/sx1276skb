using System;

namespace SemtechLib.General.Events
{
	public sealed class ByteArrayEventArg : EventArgs
	{
        public byte[] Value { get; }

        public ByteArrayEventArg(byte[] value)
		{
			Value = value;
		}
	}
}
