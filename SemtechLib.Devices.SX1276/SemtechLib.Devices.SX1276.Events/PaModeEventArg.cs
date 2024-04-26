using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class PaModeEventArg(PaSelectEnum value) : EventArgs
	{
        public PaSelectEnum Value { get; } = value;
	}
}
