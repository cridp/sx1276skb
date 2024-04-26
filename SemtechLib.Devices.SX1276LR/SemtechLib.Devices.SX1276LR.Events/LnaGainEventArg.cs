using System;
using SemtechLib.Devices.SX1276LR.Enumerations;

namespace SemtechLib.Devices.SX1276LR.Events
{
	public sealed class LnaGainEventArg(LnaGainEnum value) : EventArgs
	{
        public LnaGainEnum Value { get; } = value;
	}
}
