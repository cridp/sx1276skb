using System;
using System.ComponentModel;
using System.IO;
using SemtechLib.Devices.Common.Events;
using SemtechLib.General;

namespace SemtechLib.Devices.Common.Interfaces
{
	public interface IDevice : INotifyPropertyChanged, IDisposable
	{
		event EventHandler Connected;

		event EventHandler Disconected;

		event SemtechLib.General.Events.ErrorEventHandler Error;

		event PacketStatusEventHandler PacketHandlerReceived;

		event EventHandler PacketHandlerStarted;

		event EventHandler PacketHandlerStoped;

		event PacketStatusEventHandler PacketHandlerTransmitted;

		string DeviceName { get; }

		Version FwVersion { get; set; }

		bool IsDebugOn { get; set; }

		bool IsOpen { get; set; }

		bool IsPacketHandlerStarted { get; }

		bool Monitor { get; set; }

		RegisterCollection Registers { get; set; }

		bool Test { get; set; }

		Version Version { get; set; }

		bool Close();

		bool Open();

		void OpenConfig(ref FileStream stream);

		void ProcessWinMessage(int msg, IntPtr wParam, IntPtr lParam);

		void ReadRegisters();

		void Reset();

		void SaveConfig(ref FileStream stream);

		void SetNotificationWindowHandle(IntPtr handle, bool isWpfApplication);
	}
}
