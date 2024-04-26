using System;

namespace SemtechLib.Devices.Common.Events
{
	public sealed class PacketStatusEventArg(int number, int max) : EventArgs
	{
		public int Number { get; } = number;

        public int Max => max;
	}
}
