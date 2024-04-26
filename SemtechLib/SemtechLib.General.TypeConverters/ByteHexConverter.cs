using System;
using System.ComponentModel;
using System.Globalization;

namespace SemtechLib.General.TypeConverters
{
	public sealed class ByteHexConverter : ByteConverter
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
					if ((text.StartsWith("0x", ignoreCase: true, info) && text.Length <= 4) || text.Length <= 2)
					{
						var b = Convert.ToByte(text, 16);
						return b;
					}
				}
				catch
				{
				}
				throw new ArgumentException("Can not convert '" + text + "' to type ByteHexConverter");
			}
			return base.ConvertFrom(context, info, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
		{
			if (destType == typeof(string) && value is byte b)
			{
				return "0x" + b.ToString("X02");
			}
			return base.ConvertTo(context, culture, value, destType);
		}
	}
}
