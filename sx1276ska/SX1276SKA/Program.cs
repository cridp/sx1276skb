using System;
using System.Windows.Forms;

namespace SX1276SKA
{
	internal static class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			var testMode = false;
			if (args.Length == 1)
			{
				foreach (var text in args)
				{
					if (text == "-test")
					{
						testMode = true;
					}
				}
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(defaultValue: true);
			Application.Run(new MainForm(testMode));
		}
	}
}
