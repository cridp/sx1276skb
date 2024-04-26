using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Interop;
using Microsoft.Win32.SafeHandles;
using SemtechLib.Devices.Common.Events;

namespace SemtechLib.Devices.Common.Hid
{
	public sealed class HidDevice : IDisposable, INotifyPropertyChanged
	{
		private struct DeviceData(string name, string manufacturer, string product)
		{
            public string Name { get; } = name;

            public string Product { get; } = product;

            public string Manufacturer { get; } = manufacturer;
        }
		private readonly object syncObject = new();

		private readonly UsbDetector usbDetector;

		private string deviceID;

		private List<DeviceData> devicesList;

		private SafeFileHandle deviceStream;

		private FileStream fileStream;

		private Guid guid = new(0x4D1E55B2u, 0xF16F, 0x11CF, 0x88, 0xCB, 0x0, 0x11, 0x11, 0x0, 0x0, 0x30);

		private bool isOpen;

		private string Name = string.Empty;

		public HidDevice(int vendorId, int productId, string product)
		{
			VendorId = vendorId;
			this.ProductId = productId;
			this.Product = product;
			deviceID = string.Format(CultureInfo.InvariantCulture, "vid_{0:x4}&pid_{1:x4}", vendorId, productId);
			usbDetector = new UsbDetector();
			usbDetector.StateChanged += UsbDetector_StateChanged;
		}

		public event EventHandler Closed;

        public event EventHandler Opened;

		public event PropertyChangedEventHandler PropertyChanged;

		public bool IsOpen
		{
			get => isOpen;
			private set
			{
				isOpen = value;
				OnPropertyChanged(nameof(IsOpen));
			}
		}

        public string Manufacturer { get; } = string.Empty;

        private string Product { get; }

		private int ProductId { get; }

		private int VendorId { get; }

		public bool Close()
		{
			OnUnattached(Name);
			return !IsOpen;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public bool Open()
		{
			OnAttached(string.Empty);
			return IsOpen;
		}

		public void ProcessWinMessage(int msg, IntPtr wParam, IntPtr lParam)
		{
			usbDetector.ProcessWinMessage(msg, wParam, lParam);
		}

		public void RegisterNotification(IntPtr handle, bool isWpfApplication = false)
		{
			if (isWpfApplication)
			{
				var hwndSource = HwndSource.FromHwnd(handle);
				HwndSourceHook hook = NotificationHandler;
				hwndSource?.AddHook(hook);
			}
			usbDetector.RegisterNotification(handle);
		}

		public bool TxRxCommand(byte command, byte[] outData, ref byte[] inData)
		{
			var array = new byte[65];
			var array2 = new byte[65];
			try
			{
				lock (syncObject)
				{
					if (!IsOpen) return false;
					if (!fileStream.CanWrite && !fileStream.CanRead)
					{
						return false;
					}
					for (var num = 0u; num < array.Length; num++)
					{
						array[num] = byte.MaxValue;
					}
					array[0] = 0;
					array[1] = command;
					if (outData != null)
					{
						Array.Copy(outData, 0, array, 0x2, outData.Length);
					}
					fileStream.Write(array, 0x0, array.Length);
					if (inData == null)
					{
						return true;
					}
					array2[0x0] = 0x0;
					if (fileStream.Read(array2, 0x0, array2.Length) <= 0x0 || array2[0x1] != command) return false;
					Array.Copy(array2, 0x2, inData, 0x0, inData.Length);
					return true;
				}
			}
			catch (IOException ex)
			{
				Console.WriteLine(ex.Message);
				OnUnattached(Name);
				return false;
			}
			catch (Exception ex2)
			{
				Console.WriteLine(ex2.Message);
				return false;
			}
		}

		public void UnregisterNotification()
		{
			usbDetector.UnregisterNotification();
		}

		private static string GetDevicePath(
			IntPtr DeviceInfoTable,
			ref NativeMethods.SP_DEVICE_INTERFACE_DATA InterfaceDataStructure)
		{
			var structure = new NativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA();
			var RequiredSize = 0x0;
			structure.CbSize = Marshal.SizeOf((object) structure);
			NativeMethods.SetupDiGetDeviceInterfaceDetail(DeviceInfoTable, ref InterfaceDataStructure, IntPtr.Zero, 0x0, ref RequiredSize, IntPtr.Zero);
			
			var num = Marshal.AllocHGlobal(RequiredSize);
			structure.CbSize = IntPtr.Size != 0x8 ? 0x4 + Marshal.SystemDefaultCharSize : 0x8;
			Marshal.StructureToPtr((object) structure, num, false);
			if (NativeMethods.SetupDiGetDeviceInterfaceDetail(DeviceInfoTable, ref InterfaceDataStructure, num, RequiredSize, IntPtr.Zero, IntPtr.Zero))
			{
				var stringUni = Marshal.PtrToStringUni(new IntPtr(num.ToInt64() + 0x4));
				Marshal.FreeHGlobal(num);
				return stringUni;
			}
			Marshal.FreeHGlobal(num);
			return string.Empty;
		}

		private static void OnReadCompletion(IAsyncResult asyncResult)
		{
			((ManualResetEvent)asyncResult.AsyncState).Set();
		}

		private static void OnWriteCompletion(IAsyncResult asyncResult)
		{
			((ManualResetEvent)asyncResult.AsyncState).Set();
		}

		private static List<DeviceData> PopulateDeviceList(int vendorId, int productId)
		{
			SafeFileHandle safeFileHandle = null;
			var list = new List<DeviceData>();
			NativeMethods.HidD_GetHidGuid(out var gHid);
			var intPtr = NativeMethods.SetupDiGetClassDevs(ref gHid, IntPtr.Zero, IntPtr.Zero, 0x12);
			var value = string.Format(CultureInfo.InvariantCulture, "vid_{0:x4}&pid_{1:x4}", vendorId, productId);
			Console.WriteLine("Searching for" + value);
			try
			{
				var DeviceInterfaceData = default(NativeMethods.SP_DEVICE_INTERFACE_DATA);
				DeviceInterfaceData.CbSize = Marshal.SizeOf((object)DeviceInterfaceData);
				var num = 0x0;
				while (NativeMethods.SetupDiEnumDeviceInterfaces(intPtr, IntPtr.Zero, ref gHid, num++, ref DeviceInterfaceData))
				{
					var devicePath = GetDevicePath(intPtr, ref DeviceInterfaceData);
					if (devicePath.IndexOf(value, StringComparison.Ordinal) < 0x0)
					{
						// Console.WriteLine(devicePath);
						continue;
					}
					safeFileHandle?.Close();
					safeFileHandle = NativeMethods.CreateFile(devicePath, -0x40000000, 0x0, IntPtr.Zero, 0x3, 0x0, IntPtr.Zero);
					if (Marshal.GetLastWin32Error() != 0x0)
					{
						// Console.WriteLine(devicePath.ToString());
						continue;
					}
					var stringBuilder = new StringBuilder();
					if (!NativeMethods.HidD_GetManufacturerString(safeFileHandle, stringBuilder, 0xFF))
					{
						Console.WriteLine(stringBuilder.ToString());
						continue;
					}
					var stringBuilder2 = new StringBuilder();
					if (NativeMethods.HidD_GetProductString(safeFileHandle, stringBuilder2, 0xFF))
					{
						list.Add(new DeviceData(devicePath, stringBuilder.ToString(), stringBuilder2.ToString()));
					} else 
					{						
						Console.WriteLine(stringBuilder2.ToString());
					}
				}
				return list;
			}
			finally
			{
				safeFileHandle?.Close();
				NativeMethods.SetupDiDestroyDeviceInfoList(intPtr);
			}
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				usbDetector.UnregisterNotification();
			}
		}

		private IntPtr NotificationHandler(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			return usbDetector.NotificationHandler(hwnd, msg, wParam, lParam);
		}

		private void OnAttached(string name)
		{
			devicesList = PopulateDeviceList(VendorId, ProductId);
			foreach (var devices in devicesList)
			{
				if (IsOpen || (name.ToLower() != devices.Name && name != string.Empty) ||
				    Product != devices.Product) continue;
				deviceStream = NativeMethods.CreateFile(devices.Name, -0x40000000, 0x3, IntPtr.Zero, 0x3, 0x0, IntPtr.Zero);
				if (Marshal.GetLastWin32Error() != 0x0) continue;
				fileStream = new FileStream(deviceStream, FileAccess.ReadWrite, 0x41, isAsync: false);
				Name = devices.Name;
				IsOpen = true;
				OnOpened();
				break;
			}
		}

		private void OnClosed()
		{
            Closed?.Invoke(this, EventArgs.Empty);
        }

		private void OnOpened()
		{
            Opened?.Invoke(this, EventArgs.Empty);
        }

		private void OnPropertyChanged(string propName)
		{
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

		private void OnUnattached(string name)
		{
			if (!IsOpen || name.ToLower() != Name) return;
			if (fileStream != null)
			{
				fileStream.Close();
				Console.WriteLine("HidDevice: filestream closed");
			}
			if (deviceStream is { IsInvalid: false })
			{
				deviceStream.Close();
				Console.WriteLine("HidDevice: devicestream closed");
			}
			Name = string.Empty;
			IsOpen = false;
			OnClosed();
		}

		private int ReadAsync(byte[] array, int offset, int count, int timeout)
		{
			var manualResetEvent = new ManualResetEvent(initialState: false);
			var asyncResult = fileStream.BeginRead(array, offset, count, OnReadCompletion, manualResetEvent);
			manualResetEvent.WaitOne(timeout, exitContext: false);
			return asyncResult.IsCompleted ? fileStream.EndRead(asyncResult) : 0x0;
		}

		private void UsbDetector_StateChanged(object sender, DeviceStateEventArg e)
		{
			switch (e.State)
			{
			case DeviceState.Attached:
				OnAttached(e.Name);
				break;
			case DeviceState.Unattached:
				OnUnattached(e.Name);
				break;
			}
		}

		private void WriteAsync(byte[] array, int offset, int count, int timeout)
		{
			var manualResetEvent = new ManualResetEvent(initialState: false);
			var asyncResult = fileStream.BeginWrite(array, offset, count, OnWriteCompletion, manualResetEvent);
			manualResetEvent.WaitOne(timeout, exitContext: false);
			if (!asyncResult.IsCompleted) throw new Exception(message: "Write timeout");
			fileStream.EndWrite(asyncResult);
			
		}
	}
}
