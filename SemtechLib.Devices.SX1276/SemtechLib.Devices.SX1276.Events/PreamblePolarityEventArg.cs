using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class PreamblePolarityEventArg(PreamblePolarityEnum value) : EventArgs
	{
        public PreamblePolarityEnum Value { get; } = value;
	}
}
