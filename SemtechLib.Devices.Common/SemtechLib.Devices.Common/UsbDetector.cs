using System;
using System.Runtime.InteropServices;
using SemtechLib.Devices.Common.Events;

namespace SemtechLib.Devices.Common
{
	public sealed class UsbDetector
	{
		private readonly Guid USBClassID = new(0x4D1E55B2u, 0xF16F, 0x11CF, 0x88, 0xCB, 0x0, 0x11, 0x11, 0x0, 0x0, 0x30);

		private IntPtr handle = IntPtr.Zero;

		public event DeviceStateEventHandler StateChanged;

		public IntPtr NotificationHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
		{
			ProcessWinMessage(msg, wParam, lParam);
			return IntPtr.Zero;
		}

		public void ProcessWinMessage(int msg, IntPtr wParam, IntPtr lParam)
		{
			if (msg != NativeMethods.WM_DEVICECHANGE) return;
			Console.WriteLine("UsbDetector\t:\tmsg = 0x{0:X} - wParam = 0x{1:X} - lParam = 0x{2:X}", msg, wParam.ToInt32(), lParam.ToInt64());
			switch (wParam.ToInt32())
			{
				case NativeMethods.DBT_DEVICEARRIVAL:
					Console.WriteLine("UsbDetector\t:Device arrival");
					Console.WriteLine("DEVICE INTERFACE: Arrived");
					OnStateChanged(GetDeviceName(lParam), DeviceState.Attached);
					break;
				case NativeMethods.DBT_DEVICEREMOVECOMPLETE:
					Console.WriteLine("UsbDetector\t:Device remove complete");
					Console.WriteLine("DEVICE INTERFACE: Removed");
					OnStateChanged(GetDeviceName(lParam), DeviceState.Unattached);
					break;
				case NativeMethods.DBT_DEVICEREMOVEPENDING:
					Console.WriteLine("UsbDetector\t:Device remove pending");
					break;
				case NativeMethods.DBT_CONFIGCHANGED:
					Console.WriteLine("UsbDetector\t:Device config changed");
					break;
			}
		}

		public IntPtr RegisterNotification(IntPtr handleN)
		{
			handle = handleN;
			var dEV_BROADCAST_DEVICEINTERFACE = default(NativeMethods.DEV_BROADCAST_DEVICEINTERFACE);
			dEV_BROADCAST_DEVICEINTERFACE.dbcc_devicetype = 5;
			dEV_BROADCAST_DEVICEINTERFACE.dbcc_size = Marshal.SizeOf((object)dEV_BROADCAST_DEVICEINTERFACE);
			dEV_BROADCAST_DEVICEINTERFACE.dbcc_reserved = 0;
			dEV_BROADCAST_DEVICEINTERFACE.dbcc_classguid = USBClassID;
            var zero = Marshal.AllocHGlobal(Marshal.SizeOf((object)dEV_BROADCAST_DEVICEINTERFACE));
            Marshal.StructureToPtr((object)dEV_BROADCAST_DEVICEINTERFACE, zero, fDeleteOld: false);
			return NativeMethods.RegisterDeviceNotification(handleN, zero, 0x0);
		}

		public bool UnregisterNotification()
		{
			return NativeMethods.UnregisterDeviceNotification(handle);
		}

		private string GetDeviceName(IntPtr lParam)
		{
			var result = string.Empty;
			var dEV_BROADCAST_HDR = (NativeMethods.DEV_BROADCAST_HDR)Marshal.PtrToStructure(lParam, typeof(NativeMethods.DEV_BROADCAST_HDR));
			if (dEV_BROADCAST_HDR.Dbch_devicetype != 5) return result;
			var num = Convert.ToInt32((dEV_BROADCAST_HDR.Dbch_size - 0x20) / 0x2);
			var dEV_BROADCAST_DEVICEINTERFACE = default(NativeMethods.DEV_BROADCAST_DEVICEINTERFACE);
			dEV_BROADCAST_DEVICEINTERFACE.dbcc_name = new char[num + 1];
			result = new string(((NativeMethods.DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(lParam, typeof(NativeMethods.DEV_BROADCAST_DEVICEINTERFACE))).dbcc_name, 0x0, num);
			return result;
		}

		private void OnStateChanged(string name, DeviceState state)
		{
            StateChanged?.Invoke(this, new DeviceStateEventArg(name, state));
        }
	}
}
