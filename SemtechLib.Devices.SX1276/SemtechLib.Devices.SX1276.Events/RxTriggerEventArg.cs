using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class RxTriggerEventArg(RxTriggerEnum value) : EventArgs
	{
        public RxTriggerEnum Value { get; } = value;
	}
}
