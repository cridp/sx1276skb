using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class FromStartEventArg(FromStart value) : EventArgs
	{
        public FromStart Value { get; } = value;
	}
}
