using System;
using System.Runtime.InteropServices;

namespace SemtechLib.General.Events
{
	[Serializable]
	[ComVisible(true)]
	public delegate void Int16EventHandler(object sender, Int16EventArg e);
}
