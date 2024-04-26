using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class FromIdleEventArg(FromIdle value) : EventArgs
	{
        public FromIdle Value { get; } = value;
	}
}
