using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class TempThresholdEventArg(TempThresholdEnum value) : EventArgs
	{
        public TempThresholdEnum Value { get; } = value;
	}
}
