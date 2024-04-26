using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class OokPeakThreshDecEventArg(OokPeakThreshDecEnum value) : EventArgs
	{
        public OokPeakThreshDecEnum Value { get; } = value;
	}
}
