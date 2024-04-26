using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class AutoRestartRxEventArg(AutoRestartRxEnum value) : EventArgs
	{
        public AutoRestartRxEnum Value { get; } = value;
	}
}
