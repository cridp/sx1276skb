using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Fusionbird.FusionToolkit
{
	internal sealed class NativeMethods
	{
		public enum CustomDrawDrawStage
		{
			CDDS_ITEM = 65536,
			CDDS_ITEMPOSTERASE = 65540,
			CDDS_ITEMPOSTPAINT = 65538,
			CDDS_ITEMPREERASE = 65539,
			CDDS_ITEMPREPAINT = 65537,
			CDDS_POSTERASE = 4,
			CDDS_POSTPAINT = 2,
			CDDS_PREERASE = 3,
			CDDS_PREPAINT = 1,
			CDDS_SUBITEM = 131072
		}

		public enum CustomDrawItemState
		{
			CDIS_CHECKED = 8,
			CDIS_DEFAULT = 32,
			CDIS_DISABLED = 4,
			CDIS_FOCUS = 16,
			CDIS_GRAYED = 2,
			CDIS_HOT = 64,
			CDIS_INDETERMINATE = 256,
			CDIS_MARKED = 128,
			CDIS_SELECTED = 1,
			CDIS_SHOWKEYBOARDCUES = 512
		}

		public enum CustomDrawReturnFlags
		{
			CDRF_DODEFAULT = 0,
			CDRF_NEWFONT = 2,
			CDRF_NOTIFYITEMDRAW = 32,
			CDRF_NOTIFYPOSTERASE = 64,
			CDRF_NOTIFYPOSTPAINT = 16,
			CDRF_NOTIFYSUBITEMDRAW = 32,
			CDRF_SKIPDEFAULT = 4
		}

		public enum TrackBarCustomDrawPart
		{
			TBCD_CHANNEL = 3,
			TBCD_THUMB = 2,
			TBCD_TICS = 1
		}

		public enum TrackBarParts
		{
			TKP_THUMB = 3,
			TKP_THUMBBOTTOM = 4,
			TKP_THUMBLEFT = 7,
			TKP_THUMBRIGHT = 8,
			TKP_THUMBTOP = 5,
			TKP_THUMBVERT = 6,
			TKP_TICS = 9,
			TKP_TICSVERT = 10,
			TKP_TRACK = 1,
			TKP_TRACKVERT = 2
		}

		public struct DLLVERSIONINFO
		{
			public int cbSize;

			public int dwMajorVersion;

			public int dwMinorVersion;

			public int dwBuildNumber;

			public int dwPlatformID;
		}

		public struct NMCUSTOMDRAW
		{
			public NMHDR hdr;

			public CustomDrawDrawStage dwDrawStage;

			public IntPtr hdc;

			public RECT rc;

			public IntPtr dwItemSpec;

			public CustomDrawItemState uItemState;

			public IntPtr lItemlParam;
		}

		public struct NMHDR
		{
			public IntPtr HWND;

			public int idFrom;

			public int code;

			public override string ToString()
			{
				return string.Format(CultureInfo.InvariantCulture, "Hwnd: {0}, ControlID: {1}, Code: {2}", new object[3] { HWND, idFrom, code });
			}
		}

		public struct RECT
		{
			public int Left;

			public int Top;

			public int Right;

			public int Bottom;

			public RECT(Rectangle rect)
			{
				this = default(RECT);
				Left = rect.Left;
				Top = rect.Top;
				Right = rect.Right;
				Bottom = rect.Bottom;
			}

			public override string ToString()
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", Left, Top, Right, Bottom);
			}

			public Rectangle ToRectangle()
			{
				return Rectangle.FromLTRB(Left, Top, Right, Bottom);
			}
		}

		public const int NM_CUSTOMDRAW = -12;

		public const int NM_FIRST = 0;

		public const int S_OK = 0;

		public const int TMT_COLOR = 204;

		private NativeMethods()
		{
		}

		[DllImport("UxTheme.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int CloseThemeData(IntPtr hTheme);

		[DllImport("Comctl32.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DllGetVersion")]
		public static extern int CommonControlsGetVersion(ref DLLVERSIONINFO pdvi);

		[DllImport("UxTheme.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int DrawThemeBackground(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, ref RECT pRect, ref RECT pClipRect);

		[DllImport("UxTheme.dll", CallingConvention = CallingConvention.Cdecl)]
		public static extern int GetThemeColor(IntPtr hTheme, int iPartId, int iStateId, int iPropId, ref int pColor);

		[DllImport("UxTheme.dll", CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsAppThemed();

		[DllImport("UxTheme.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		public static extern IntPtr OpenThemeData(IntPtr hwnd, string pszClassList);
	}
}
