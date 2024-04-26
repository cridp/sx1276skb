using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class FromTransmitEventArg(FromTransmit value) : EventArgs
	{
        public FromTransmit Value { get; } = value;
	}
}
