using System;
using SemtechLib.Devices.SX1276LR.Enumerations;

namespace SemtechLib.Devices.SX1276LR.Events
{
	public sealed class DioMappingEventArg(byte id, DioMappingEnum value) : EventArgs
	{
		public byte Id { get; } = id;

        public DioMappingEnum Value => value;
	}
}
