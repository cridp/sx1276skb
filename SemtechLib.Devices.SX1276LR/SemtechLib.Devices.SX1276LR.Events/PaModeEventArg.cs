using System;
using SemtechLib.Devices.SX1276LR.Enumerations;

namespace SemtechLib.Devices.SX1276LR.Events
{
	public sealed class PaModeEventArg(PaSelectEnum value) : EventArgs
	{
        public PaSelectEnum Value { get; } = value;
	}
}
