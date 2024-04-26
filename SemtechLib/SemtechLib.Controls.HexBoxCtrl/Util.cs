using System.Diagnostics;

namespace SemtechLib.Controls.HexBoxCtrl
{
	internal static class Util
	{
        public static bool DesignMode { get; private set; }

        static Util()
		{
			DesignMode = Process.GetCurrentProcess().ProcessName.ToLower() == "devenv";
		}
	}
}
