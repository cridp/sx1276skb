using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class LnaGainEventArg(LnaGainEnum value) : EventArgs
	{
        public LnaGainEnum Value { get; } = value;
	}
}
