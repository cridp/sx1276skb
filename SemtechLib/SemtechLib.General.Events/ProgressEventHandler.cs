using System;
using System.Runtime.InteropServices;

namespace SemtechLib.General.Events
{
	[Serializable]
	[ComVisible(true)]
	public delegate void ProgressEventHandler(object sender, ProgressEventArg e);
}
