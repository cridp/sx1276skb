using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class ListenEndEventArg(ListenEndEnum value) : EventArgs
	{
        public ListenEndEnum Value { get; } = value;
	}
}
