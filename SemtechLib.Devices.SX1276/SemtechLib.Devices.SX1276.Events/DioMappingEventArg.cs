using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class DioMappingEventArg(byte id, DioMappingEnum value) : EventArgs
	{
		public byte Id { get; } = id;

        public DioMappingEnum Value => value;
	}
}
