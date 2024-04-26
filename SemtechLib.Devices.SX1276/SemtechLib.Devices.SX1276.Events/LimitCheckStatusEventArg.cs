using System;
using SemtechLib.Devices.SX1276.Enumerations;

namespace SemtechLib.Devices.SX1276.Events
{
	public sealed class LimitCheckStatusEventArg(LimitCheckStatusEnum status, string message) : EventArgs
	{
		public LimitCheckStatusEnum Status { get; } = status;

        public string Message => message;
	}
}
