using System;
using System.Runtime.InteropServices;

namespace SemtechLib.General.Events
{
	[Serializable]
	[ComVisible(true)]
	public delegate void ByteArrayEventHandler(object sender, ByteArrayEventArg e);
}
