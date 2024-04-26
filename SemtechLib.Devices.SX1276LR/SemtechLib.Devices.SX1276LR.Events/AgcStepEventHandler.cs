using System;
using System.Runtime.InteropServices;

namespace SemtechLib.Devices.SX1276LR.Events
{
	[Serializable]
	[ComVisible(true)]
	public delegate void AgcStepEventHandler(object sender, AgcStepEventArg e);
}
