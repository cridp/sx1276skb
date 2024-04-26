using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class IntermediateModeEventArg(IntermediateModeEnum value) : EventArgs
	{
        public IntermediateModeEnum Value { get; } = value;
	}
}
