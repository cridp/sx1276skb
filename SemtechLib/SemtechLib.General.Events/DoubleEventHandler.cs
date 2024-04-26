using System;
using System.Runtime.InteropServices;

namespace SemtechLib.General.Events
{
	[Serializable]
	[ComVisible(true)]
	public delegate void DoubleEventHandler(object sender, DoubleEventArg e);
}
