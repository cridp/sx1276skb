using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class RfPaSwitchSelEventArg(RfPaSwitchSelEnum value) : EventArgs
	{
        public RfPaSwitchSelEnum Value { get; } = value;
	}
}
