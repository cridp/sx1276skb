using System;

namespace SemtechLib.Devices.Common.Events
{
	public sealed class DeviceStateEventArg(string name, DeviceState state) : EventArgs
	{
		public string Name { get; } = name;

        public DeviceState State => state;
	}
}
