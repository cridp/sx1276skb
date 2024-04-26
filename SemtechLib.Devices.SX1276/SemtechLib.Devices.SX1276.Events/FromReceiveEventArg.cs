using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class FromReceiveEventArg(FromReceive value) : EventArgs
	{
        public FromReceive Value { get; } = value;
	}
}
