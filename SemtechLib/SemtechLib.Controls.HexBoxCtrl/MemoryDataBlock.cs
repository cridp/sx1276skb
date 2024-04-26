using System;

namespace SemtechLib.Controls.HexBoxCtrl
{
	internal sealed class MemoryDataBlock : DataBlock
	{
        public override long Length => Data.LongLength;

        public byte[] Data { get; private set; }

        public MemoryDataBlock(byte data)
		{
			Data = new byte[1] { data };
		}

		public MemoryDataBlock(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}
			Data = (byte[])data.Clone();
		}

		public void AddByteToEnd(byte value)
		{
			var array = new byte[Data.LongLength + 1];
			Data.CopyTo(array, 0);
			array[array.LongLength - 1] = value;
			Data = array;
		}

		public void AddByteToStart(byte value)
		{
			var array = new byte[Data.LongLength + 1];
			array[0] = value;
			Data.CopyTo(array, 1);
			Data = array;
		}

		public void InsertBytes(long position, byte[] data)
		{
			var array = new byte[Data.LongLength + data.LongLength];
			if (position > 0)
			{
				Array.Copy(Data, 0L, array, 0L, position);
			}
			Array.Copy(data, 0L, array, position, data.LongLength);
			if (position < Data.LongLength)
			{
				Array.Copy(Data, position, array, position + data.LongLength, Data.LongLength - position);
			}
			Data = array;
		}

		public override void RemoveBytes(long position, long count)
		{
			var array = new byte[Data.LongLength - count];
			if (position > 0)
			{
				Array.Copy(Data, 0L, array, 0L, position);
			}
			if (position + count < Data.LongLength)
			{
				Array.Copy(Data, position + count, array, position, array.LongLength - position);
			}
			Data = array;
		}
	}
}
