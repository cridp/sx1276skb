using System;
using System.ComponentModel;
using System.Globalization;

namespace SemtechLib.General.TypeConverters
{
	public sealed class UInt32HexConverter : UInt32Converter
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
					if ((text.StartsWith("0x", ignoreCase: true, info) && text.Length <= 10) || text.Length <= 8)
					{
						var num = Convert.ToUInt32(text, 16);
						return num;
					}
				}
				catch
				{
				}
				throw new ArgumentException("Can not convert '" + text + "' to type UInt32HexConverter");
			}
			return base.ConvertFrom(context, info, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
		{
			if (destType == typeof(string) && value is uint num)
			{
				return "0x" + num.ToString("X08");
			}
			return base.ConvertTo(context, culture, value, destType);
		}
	}
}
