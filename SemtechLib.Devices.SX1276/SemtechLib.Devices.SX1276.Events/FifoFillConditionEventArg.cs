using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class FifoFillConditionEventArg(FifoFillConditionEnum value) : EventArgs
	{
        public FifoFillConditionEnum Value { get; } = value;
	}
}
