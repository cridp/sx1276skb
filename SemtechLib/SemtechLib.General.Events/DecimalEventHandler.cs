using System;
using System.Runtime.InteropServices;

namespace SemtechLib.General.Events
{
	[Serializable]
	[ComVisible(true)]
	public delegate void DecimalEventHandler(object sender, DecimalEventArg e);
}
