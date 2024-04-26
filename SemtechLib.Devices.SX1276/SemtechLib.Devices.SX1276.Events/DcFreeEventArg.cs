using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class DcFreeEventArg(DcFreeEnum value) : EventArgs
	{
        public DcFreeEnum Value { get; } = value;
	}
}
