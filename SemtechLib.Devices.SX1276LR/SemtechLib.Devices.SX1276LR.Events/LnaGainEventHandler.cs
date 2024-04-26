using System;
using System.Runtime.InteropServices;

namespace SemtechLib.Devices.SX1276LR.Events
{
	[Serializable]
	[ComVisible(true)]
	public delegate void LnaGainEventHandler(object sender, LnaGainEventArg e);
}
