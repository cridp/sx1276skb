using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class LowBatTrimEventArg(LowBatTrimEnum value) : EventArgs
	{
        public LowBatTrimEnum Value { get; } = value;
	}
}
