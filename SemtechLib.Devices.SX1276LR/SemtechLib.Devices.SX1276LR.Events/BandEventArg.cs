using System;
using SemtechLib.Devices.SX1276LR.Enumerations;

namespace SemtechLib.Devices.SX1276LR.Events
{
	public sealed class BandEventArg(BandEnum value) : EventArgs
	{
        public BandEnum Value { get; } = value;
	}
}
