using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class FromRxTimeoutEventArg(FromRxTimeout value) : EventArgs
	{
        public FromRxTimeout Value { get; } = value;
	}
}
