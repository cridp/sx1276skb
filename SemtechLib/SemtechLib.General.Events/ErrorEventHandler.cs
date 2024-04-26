using System;
using System.Runtime.InteropServices;

namespace SemtechLib.General.Events
{
	[Serializable]
	[ComVisible(true)]
	public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);
}
