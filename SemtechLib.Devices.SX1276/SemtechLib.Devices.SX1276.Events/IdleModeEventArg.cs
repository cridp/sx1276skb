using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class IdleModeEventArg(IdleMode value) : EventArgs
	{
        public IdleMode Value { get; } = value;
	}
}
