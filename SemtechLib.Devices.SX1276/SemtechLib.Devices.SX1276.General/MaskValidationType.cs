using System;
using System.Globalization;

namespace SemtechLib.Devices.SX1276.General
{
	public sealed class MaskValidationType
	{
        private byte[] arrayValue;

        private static int Length { get; set; } = 1;

        public static MaskValidationType InvalidMask => new(new byte[Length]);

		public string StringValue
		{
			get => ToString();
			set
			{
				try
				{
					var array = value.Split('-');
					arrayValue = new byte[array.Length];
					for (var i = 0; i < array.Length; i++)
					{
						arrayValue[i] = Convert.ToByte(array[i], 16);
					}
				}
				catch
				{
				}
				finally
				{
					Length = arrayValue.Length;
				}
			}
		}

		public byte[] ArrayValue
		{
			get => arrayValue;
			set
			{
				arrayValue ??= new byte[1];
				if (value == null)
				{
					throw new ArgumentNullException("The array cannot be null.");
				}
				if (value.Length < 1 && value.Length > 8)
				{
					throw new ArgumentException("Array should have as size comprized between 1 and 8.");
				}
				if (arrayValue.Length != value.Length)
				{
					Array.Resize(ref arrayValue, value.Length);
				}
				Array.Copy(value, arrayValue, value.Length);
				Length = value.Length;
			}
		}

		public MaskValidationType()
		{
			arrayValue = new byte[1];
			Length = 1;
		}

		public MaskValidationType(string stringValue)
		{
			StringValue = stringValue;
		}

		public MaskValidationType(byte[] array)
		{
			ArrayValue = array;
		}

		private static void doParsing(string s, out byte[] bytes)
		{
			s = s.Replace(" ", "");
			var array = s.Split('-');
			bytes = new byte[array.Length];
			try
			{
				var num = 0;
				var array2 = array;
				foreach (var value in array2)
				{
					bytes[num] = Convert.ToByte(value, 16);
					num++;
				}
			}
			catch
			{
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The provided string {0} is not valid", new object[1] { s }));
			}
		}

		public static MaskValidationType Parse(string s)
		{
			doParsing(s, out var bytes);
			return new MaskValidationType(bytes);
		}

		public override string ToString()
		{
			var text = "";
			int i;
			for (i = 0; i < arrayValue.Length - 1; i++)
			{
				text = text + arrayValue[i].ToString("X02", CultureInfo.CurrentCulture) + "-";
			}
			return text + arrayValue[i].ToString("X02", CultureInfo.CurrentCulture);
		}
	}
}
