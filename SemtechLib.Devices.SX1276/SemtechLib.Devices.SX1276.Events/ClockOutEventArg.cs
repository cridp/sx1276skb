using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class ClockOutEventArg(ClockOutEnum value) : EventArgs
	{
        public ClockOutEnum Value { get; } = value;
	}
}
