using System;
using System.ComponentModel;
using System.Globalization;

namespace SemtechLib.General.TypeConverters
{
	public sealed class UInt16HexConverter : UInt16Converter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type t)
		{
			if (t == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, t);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo info, object value)
		{
			if (value is string text)
			{
				try
				{
					if ((text.StartsWith("0x", ignoreCase: true, info) && text.Length <= 6) || text.Length <= 4)
					{
						var num = Convert.ToUInt16(text, 16);
						return num;
					}
				}
				catch
				{
				}
				throw new ArgumentException("Can not convert '" + text + "' to type UInt16HexConverter");
			}
			return base.ConvertFrom(context, info, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
		{
			if (destType == typeof(string) && value is ushort num)
			{
				return "0x" + num.ToString("X04");
			}
			return base.ConvertTo(context, culture, value, destType);
		}
	}
}
