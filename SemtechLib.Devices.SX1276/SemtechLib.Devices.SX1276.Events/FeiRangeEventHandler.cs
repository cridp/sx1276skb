using System;
using System.Runtime.InteropServices;

namespace SemtechLib.Devices.SX1276.Events
{
	[Serializable]
	[ComVisible(true)]
	public delegate void FeiRangeEventHandler(object sender, FeiRangeEventArg e);
}
