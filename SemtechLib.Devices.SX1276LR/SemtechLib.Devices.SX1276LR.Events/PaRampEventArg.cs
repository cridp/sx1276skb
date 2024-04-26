using System;
using SemtechLib.Devices.SX1276LR.Enumerations;

namespace SemtechLib.Devices.SX1276LR.Events
{
	public sealed class PaRampEventArg(PaRampEnum value) : EventArgs
	{
        public PaRampEnum Value { get; } = value;
	}
}
