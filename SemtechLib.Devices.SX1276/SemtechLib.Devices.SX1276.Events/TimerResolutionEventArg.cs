using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class TimerResolutionEventArg(TimerResolution value) : EventArgs
	{
        public TimerResolution Value { get; } = value;
	}
}
