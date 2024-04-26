using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class FromPacketReceivedEventArg(FromPacketReceived value) : EventArgs
	{
        public FromPacketReceived Value { get; } = value;
	}
}
