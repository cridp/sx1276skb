using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class ListenCriteriaEventArg(ListenCriteriaEnum value) : EventArgs
	{
        public ListenCriteriaEnum Value { get; } = value;
	}
}
