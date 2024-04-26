using System;
using System.Windows.Forms;
using SemtechLib.General.Events;
using SemtechLib.General.Interfaces;

namespace SemtechLib.Devices.Common.Interfaces
{
	public interface IDeviceView : IDisposable, INotifyDocumentationChanged
	{
		event ErrorEventHandler Error;

		IDevice Device { get; set; }

		DockStyle Dock { get; set; }

		bool Enabled { get; set; }

		string Name { get; set; }

		int TabIndex { get; set; }

		bool Visible { get; set; }
	}
}
