using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class ListenResolEventArg(ListenResolEnum value) : EventArgs
	{
        public ListenResolEnum Value { get; } = value;
	}
}
