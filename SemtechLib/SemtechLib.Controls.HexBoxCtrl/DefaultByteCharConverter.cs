namespace SemtechLib.Controls.HexBoxCtrl
{
	public sealed class DefaultByteCharConverter : IByteCharConverter
	{
		public char ToChar(byte b)
		{
			if (b <= 31 || (b > 126 && b < 160))
			{
				return '.';
			}
			return (char)b;
		}

		public byte ToByte(char c)
		{
			return (byte)c;
		}

		public override string ToString()
		{
			return "Default";
		}
	}
}
