using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class LowPowerSelectionEventArg(LowPowerSelection value) : EventArgs
	{
        public LowPowerSelection Value { get; } = value;
	}
}
