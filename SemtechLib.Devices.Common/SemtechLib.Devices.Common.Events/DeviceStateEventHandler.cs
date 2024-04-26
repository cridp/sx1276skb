using System;
using System.Runtime.InteropServices;

namespace SemtechLib.Devices.Common.Events
{
	[Serializable]
	[ComVisible(true)]
	public delegate void DeviceStateEventHandler(object sender, DeviceStateEventArg e);
}
