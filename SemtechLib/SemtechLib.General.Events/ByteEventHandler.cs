using System;
using System.Runtime.InteropServices;

namespace SemtechLib.General.Events
{
	[Serializable]
	[ComVisible(true)]
	public delegate void ByteEventHandler(object sender, ByteEventArg e);
}
