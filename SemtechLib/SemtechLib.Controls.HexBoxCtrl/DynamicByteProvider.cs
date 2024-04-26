using System;

namespace SemtechLib.Controls.HexBoxCtrl
{
	public sealed class DynamicByteProvider : IByteProvider
	{
		private bool _hasChanges;

        public ByteCollection Bytes { get; }

        public long Length => Bytes.Count;

		public event EventHandler Changed;

		public event EventHandler LengthChanged;

		public DynamicByteProvider(byte[] data)
			: this(new ByteCollection(data))
		{
		}

		public DynamicByteProvider(ByteCollection bytes)
		{
			Bytes = bytes;
		}

		private void OnChanged(EventArgs e)
		{
			_hasChanges = true;
            Changed?.Invoke(this, e);
        }

		private void OnLengthChanged(EventArgs e)
		{
            LengthChanged?.Invoke(this, e);
        }

		public bool HasChanges()
		{
			return _hasChanges;
		}

		public void ApplyChanges()
		{
			_hasChanges = false;
		}

		public byte ReadByte(long index)
		{
			return Bytes[(int)index];
		}

		public void WriteByte(long index, byte value)
		{
			Bytes[(int)index] = value;
			OnChanged(EventArgs.Empty);
		}

		public void DeleteBytes(long index, long length)
		{
			var index2 = (int)Math.Max(0L, index);
			var count = (int)Math.Min((int)Length, length);
			Bytes.RemoveRange(index2, count);
			OnLengthChanged(EventArgs.Empty);
			OnChanged(EventArgs.Empty);
		}

		public void InsertBytes(long index, byte[] bs)
		{
			Bytes.InsertRange((int)index, bs);
			OnLengthChanged(EventArgs.Empty);
			OnChanged(EventArgs.Empty);
		}

		public bool SupportsWriteByte()
		{
			return true;
		}

		public bool SupportsInsertBytes()
		{
			return true;
		}

		public bool SupportsDeleteBytes()
		{
			return true;
		}
	}
}
