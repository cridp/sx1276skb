using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class EnterConditionEventArg(EnterConditionEnum value) : EventArgs
	{
        public EnterConditionEnum Value { get; } = value;
	}
}
