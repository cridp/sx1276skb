using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace SemtechLib.Devices.Common
{
	public static class NativeMethods
	{
		public struct SP_DEVICE_INTERFACE_DATA
		{
 //           private Guid interfaceClassGuid;

            public int CbSize { get; set; }
            public Guid InterfaceClassGuid { get; set; }
            public int Flags { get; set; }
            public IntPtr Reserved { get; set; }
        }

		public struct SP_DEVICE_INTERFACE_DETAIL_DATA
		{
            public char[] DevicePath { get; set; }
            public int CbSize { get; set; }
        }

		public struct SP_DEVINFO_DATA
		{
 //           private Guid classGuid;

            public int CbSize { get; set; }
            public Guid ClassGuid { get ; set; }
            public int DevInst { get; set; }
            public IntPtr Reserved { get; set; }
        }

		public struct DEV_BROADCAST_HDR
		{
            public int Dbch_size { get; set; }
            public int Dbch_devicetype { get; set; }
            public int Dbch_reserved { get; set; }
        }

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct DEV_BROADCAST_DEVICEINTERFACE
		{
			public int dbcc_size;

			public int dbcc_devicetype;

			public int dbcc_reserved;

			public Guid dbcc_classguid;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xFF)]
			public char[] dbcc_name;
		}

		public struct HIDD_ATTRIBUTES
		{
            public int Size { get; set; }
            public short VendorID { get; set; }
            public short ProductID { get; set; }
            public short VersionNumber { get; set; }
        }

		public const int DIGCF_PRESENT = 0x2;

		public const int DIGCF_DEVICEINTERFACE = 0x10;

		public const int FILE_ATTRIBUTE_NORMAL = 0x80;

		public const int FILE_FLAG_OVERLAPPED = 0x40000000;

		public const int INVALID_HANDLE_VALUE = -0x1;

		public const int GENERIC_READ = int.MinValue;

		public const int GENERIC_WRITE = 0x40000000;

		public const int CREATE_NEW = 0x1;

		public const int CREATE_ALWAYS = 0x2;

		public const int OPEN_EXISTING = 0x3;

		public const int FILE_SHARE_READ = 0x1;

		public const int FILE_SHARE_WRITE = 0x2;

		public const int WM_DEVICECHANGE = 0x219;

		public const int DBT_DEVICEARRIVAL = 0x8000;

		public const int DBT_DEVICEQUERYREMOVE = 0x8001;

		public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;

		public const int DBT_DEVICEREMOVEPENDING = 0x8003;

		public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

		public const int DBT_DEVICETYPESPECIFIC = 0x8005;

		public const int DBT_CUSTOMEVENT = 0x8006;

		public const int DBT_DEVNODES_CHANGED = 0x7;

		public const int DBT_QUERYCHANGECONFIG = 0x17;

		public const int DBT_CONFIGCHANGED = 0x18;

		public const int DBT_CONFIGCHANGECANCELED = 0x19;

		public const int DBT_USERDEFINED = 0xFFFF;

		public const int DBT_DEVTYP_DEVICEINTERFACE = 0x5;

		public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x0;

		public const int DEVICE_NOTIFY_SERVICE_HANDLE = 0x1;

		public const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x4;

		public const int ERROR_SUCCESS = 0x0;

		public const int ERROR_NO_MORE_ITEMS = 0x103;

		public const int SPDRP_DEVICEDESC = 0x0;

		public const int SPDRP_HARDWAREID = 0x1;

		public const int SPDRP_FRIENDLYNAME = 0xC;

		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, int Flags);

		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref Guid InterfaceClassGuid, int MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, int MemberIndex, ref SP_DEVINFO_DATA DeviceInterfaceData);

		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, int Property, ref int PropertyRegDataType, IntPtr PropertyBuffer, int PropertyBufferSize, ref int RequiredSize);

		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, ref int RequiredSize, IntPtr DeviceInfoData);

		[DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, int DeviceInterfaceDetailDataSize, IntPtr RequiredSize, IntPtr DeviceInfoData);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, int Flags);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnregisterDeviceNotification(IntPtr handle);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool WriteFile(SafeFileHandle hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, IntPtr lpOverlapped);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ReadFile(SafeFileHandle hFile, IntPtr lpBuffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, IntPtr lpOverlapped);

		[DllImport("hid.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern void HidD_GetHidGuid(out Guid gHid);

		[DllImport("hid.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool HidD_GetAttributes(SafeFileHandle HidDeviceObject, ref HIDD_ATTRIBUTES Attributes);

		[DllImport("hid.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool HidD_SetOutputReport(IntPtr HidDeviceObject, byte[] lpReportBuffer, int ReportBufferLength);

		[DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool HidD_GetProductString(SafeFileHandle hidDeviceObject, StringBuilder lpbuffer, int bufferLength);

		[DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern bool HidD_GetManufacturerString(SafeFileHandle HidDeviceObject, StringBuilder lpBuffer, int BufferLength);
	}
}
