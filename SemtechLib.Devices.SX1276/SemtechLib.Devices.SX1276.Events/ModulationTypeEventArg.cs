using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class ModulationTypeEventArg(ModulationTypeEnum value) : EventArgs
	{
        public ModulationTypeEnum Value { get; } = value;
	}
}
