using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class BandEventArg(BandEnum value) : EventArgs
	{
        public BandEnum Value { get; } = value;
	}
}
