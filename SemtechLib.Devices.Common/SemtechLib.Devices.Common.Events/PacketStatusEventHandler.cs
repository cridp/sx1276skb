using System;
using System.Runtime.InteropServices;

namespace SemtechLib.Devices.Common.Events
{
	[Serializable]
	[ComVisible(true)]
	public delegate void PacketStatusEventHandler(object sender, PacketStatusEventArg e);
}
