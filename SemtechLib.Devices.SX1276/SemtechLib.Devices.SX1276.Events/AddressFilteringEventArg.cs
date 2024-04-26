using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class AddressFilteringEventArg(AddressFilteringEnum value) : EventArgs
	{
        public AddressFilteringEnum Value { get; } = value;
	}
}
