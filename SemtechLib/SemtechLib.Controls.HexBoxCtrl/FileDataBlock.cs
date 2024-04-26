using System;

namespace SemtechLib.Controls.HexBoxCtrl
{
	internal sealed class FileDataBlock : DataBlock
	{
		private long _length;

        public long FileOffset { get; private set; }

        public override long Length => _length;

		public FileDataBlock(long fileOffset, long length)
		{
			FileOffset = fileOffset;
			_length = length;
		}

		public void SetFileOffset(long value)
		{
			FileOffset = value;
		}

		public void RemoveBytesFromEnd(long count)
		{
			if (count > _length)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}
			_length -= count;
		}

		public void RemoveBytesFromStart(long count)
		{
			if (count > _length)
			{
				throw new ArgumentOutOfRangeException(nameof(count));
			}
			FileOffset += count;
			_length -= count;
		}

		public override void RemoveBytes(long position, long count)
		{
			if (position > _length)
			{
				throw new ArgumentOutOfRangeException("position");
			}
			if (position + count > _length)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			var fileOffset = FileOffset;
			var num = _length - count - position;
			var fileOffset2 = FileOffset + position + count;
			if (position > 0 && num > 0)
			{
				FileOffset = fileOffset;
				_length = position;
				_map.AddAfter(this, new FileDataBlock(fileOffset2, num));
			}
			else if (position > 0)
			{
				FileOffset = fileOffset;
				_length = position;
			}
			else
			{
				FileOffset = fileOffset2;
				_length = num;
			}
		}
	}
}
