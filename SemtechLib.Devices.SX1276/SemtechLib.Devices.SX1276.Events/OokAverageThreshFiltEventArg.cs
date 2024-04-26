using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class OokAverageThreshFiltEventArg(OokAverageThreshFiltEnum value) : EventArgs
	{
        public OokAverageThreshFiltEnum Value { get; } = value;
	}
}
