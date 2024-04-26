using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class DataModeEventArg(DataModeEnum value) : EventArgs
	{
        public DataModeEnum Value { get; } = value;
	}
}
