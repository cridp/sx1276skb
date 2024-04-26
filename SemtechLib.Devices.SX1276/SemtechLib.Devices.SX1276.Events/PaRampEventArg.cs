using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class PaRampEventArg(PaRampEnum value) : EventArgs
	{
        public PaRampEnum Value { get; } = value;
	}
}
