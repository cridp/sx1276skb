using System;
using SemtechLib.Devices.SX1276LR.Enumerations;

namespace SemtechLib.Devices.SX1276LR.Events
{
	public sealed class ClockOutEventArg(ClockOutEnum value) : EventArgs
	{
        public ClockOutEnum Value { get; } = value;
	}
}
