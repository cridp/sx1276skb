using System;

namespace SemtechLib.Devices.SX1276LR.Events
{
	public sealed class AgcStepEventArg(byte id, byte value) : EventArgs
	{
		public byte Id { get; } = id;

        public byte Value => value;
	}
}
