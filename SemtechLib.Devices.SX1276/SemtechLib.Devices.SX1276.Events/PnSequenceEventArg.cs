using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class PnSequenceEventArg(PnSequence value) : EventArgs
	{
        public PnSequence Value { get; } = value;
	}
}
