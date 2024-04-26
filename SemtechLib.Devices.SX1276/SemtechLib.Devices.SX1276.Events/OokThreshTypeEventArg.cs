using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class OokThreshTypeEventArg(OokThreshTypeEnum value) : EventArgs
	{
        public OokThreshTypeEnum Value { get; } = value;
	}
}
