using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class FeiRangeEventArg(FeiRangeEnum value) : EventArgs
	{
        public FeiRangeEnum Value { get; } = value;
	}
}
