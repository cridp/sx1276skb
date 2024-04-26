using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class ExitConditionEventArg(ExitConditionEnum value) : EventArgs
	{
        public ExitConditionEnum Value { get; } = value;
	}
}
