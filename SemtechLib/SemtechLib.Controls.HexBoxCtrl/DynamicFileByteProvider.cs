using System;
using System.IO;

namespace SemtechLib.Controls.HexBoxCtrl
{
	public sealed class DynamicFileByteProvider : IByteProvider, IDisposable
	{
		private const int COPY_BLOCK_SIZE = 4096;

		private string _fileName;

		private Stream _stream;

		private DataMap _dataMap;
        private bool _readOnly;

        public long Length { get; private set; }

        public bool ReadOnly => _readOnly;

		public event EventHandler LengthChanged;

		public event EventHandler Changed;

		public DynamicFileByteProvider(string fileName)
			: this(fileName, readOnly: false)
		{
		}

		public DynamicFileByteProvider(string fileName, bool readOnly)
		{
			_fileName = fileName;
			if (!readOnly)
			{
				_stream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
			}
			else
			{
				_stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			}
			_readOnly = readOnly;
			ReInitialize();
		}

		public DynamicFileByteProvider(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}
			if (!stream.CanSeek)
			{
				throw new ArgumentException("stream must supported seek operations(CanSeek)");
			}
			_stream = stream;
			_readOnly = !stream.CanWrite;
			ReInitialize();
		}

		public byte ReadByte(long index)
		{
			long blockOffset;
			var dataBlock = GetDataBlock(index, out blockOffset);
			if (dataBlock is FileDataBlock fileDataBlock)
			{
				return ReadByteFromFile(fileDataBlock.FileOffset + index - blockOffset);
			}
			var memoryDataBlock = (MemoryDataBlock)dataBlock;
			return memoryDataBlock.Data[index - blockOffset];
		}

		public void WriteByte(long index, byte value)
		{
			try
			{
				var dataBlock = GetDataBlock(index, out var blockOffset);
				if (dataBlock is MemoryDataBlock memoryDataBlock)
				{
					memoryDataBlock.Data[index - blockOffset] = value;
					return;
				}
				var fileDataBlock = (FileDataBlock)dataBlock;
				if (blockOffset == index && dataBlock.PreviousBlock != null && dataBlock.PreviousBlock is MemoryDataBlock memoryDataBlock2)
				{
					memoryDataBlock2.AddByteToEnd(value);
					fileDataBlock.RemoveBytesFromStart(1L);
					if (fileDataBlock.Length == 0)
					{
						_dataMap.Remove(fileDataBlock);
					}
					return;
				}
				if (blockOffset + fileDataBlock.Length - 1 == index && dataBlock.NextBlock != null && dataBlock.NextBlock is MemoryDataBlock memoryDataBlock3)
				{
					memoryDataBlock3.AddByteToStart(value);
					fileDataBlock.RemoveBytesFromEnd(1L);
					if (fileDataBlock.Length == 0)
					{
						_dataMap.Remove(fileDataBlock);
					}
					return;
				}
				FileDataBlock fileDataBlock2 = null;
				if (index > blockOffset)
				{
					fileDataBlock2 = new FileDataBlock(fileDataBlock.FileOffset, index - blockOffset);
				}
				FileDataBlock fileDataBlock3 = null;
				if (index < blockOffset + fileDataBlock.Length - 1)
				{
					fileDataBlock3 = new FileDataBlock(fileDataBlock.FileOffset + index - blockOffset + 1, fileDataBlock.Length - (index - blockOffset + 1));
				}
				dataBlock = _dataMap.Replace(dataBlock, new MemoryDataBlock(value));
				if (fileDataBlock2 != null)
				{
					_dataMap.AddBefore(dataBlock, fileDataBlock2);
				}
				if (fileDataBlock3 != null)
				{
					_dataMap.AddAfter(dataBlock, fileDataBlock3);
				}
			}
			finally
			{
				OnChanged(EventArgs.Empty);
			}
		}

		public void InsertBytes(long index, byte[] bs)
		{
			try
			{
				var dataBlock = GetDataBlock(index, out var blockOffset);
				if (dataBlock is MemoryDataBlock memoryDataBlock)
				{
					memoryDataBlock.InsertBytes(index - blockOffset, bs);
					return;
				}
				var fileDataBlock = (FileDataBlock)dataBlock;
				if (blockOffset == index && dataBlock.PreviousBlock != null && dataBlock.PreviousBlock is MemoryDataBlock memoryDataBlock2)
				{
					memoryDataBlock2.InsertBytes(memoryDataBlock2.Length, bs);
					return;
				}
				FileDataBlock fileDataBlock2 = null;
				if (index > blockOffset)
				{
					fileDataBlock2 = new FileDataBlock(fileDataBlock.FileOffset, index - blockOffset);
				}
				FileDataBlock fileDataBlock3 = null;
				if (index < blockOffset + fileDataBlock.Length)
				{
					fileDataBlock3 = new FileDataBlock(fileDataBlock.FileOffset + index - blockOffset, fileDataBlock.Length - (index - blockOffset));
				}
				dataBlock = _dataMap.Replace(dataBlock, new MemoryDataBlock(bs));
				if (fileDataBlock2 != null)
				{
					_dataMap.AddBefore(dataBlock, fileDataBlock2);
				}
				if (fileDataBlock3 != null)
				{
					_dataMap.AddAfter(dataBlock, fileDataBlock3);
				}
			}
			finally
			{
				Length += bs.Length;
				OnLengthChanged(EventArgs.Empty);
				OnChanged(EventArgs.Empty);
			}
		}

		public void DeleteBytes(long index, long length)
		{
			try
			{
				var num = length;
				long blockOffset;
				var dataBlock = GetDataBlock(index, out blockOffset);
				while (num > 0 && dataBlock != null)
				{
					var length2 = dataBlock.Length;
					var nextBlock = dataBlock.NextBlock;
					var num2 = Math.Min(num, length2 - (index - blockOffset));
					dataBlock.RemoveBytes(index - blockOffset, num2);
					if (dataBlock.Length == 0)
					{
						_dataMap.Remove(dataBlock);
						if (_dataMap.FirstBlock == null)
						{
							_dataMap.AddFirst(new MemoryDataBlock(new byte[0]));
						}
					}
					num -= num2;
					blockOffset += dataBlock.Length;
					dataBlock = ((num > 0) ? nextBlock : null);
				}
			}
			finally
			{
				Length -= length;
				OnLengthChanged(EventArgs.Empty);
				OnChanged(EventArgs.Empty);
			}
		}

		public bool HasChanges()
		{
			if (_readOnly)
			{
				return false;
			}
			if (Length != _stream.Length)
			{
				return true;
			}
			var num = 0L;
			for (var dataBlock = _dataMap.FirstBlock; dataBlock != null; dataBlock = dataBlock.NextBlock)
			{
				if (!(dataBlock is FileDataBlock fileDataBlock))
				{
					return true;
				}
				if (fileDataBlock.FileOffset != num)
				{
					return true;
				}
				num += fileDataBlock.Length;
			}
			return num != _stream.Length;
		}

		public void ApplyChanges()
		{
			if (_readOnly)
			{
				throw new OperationCanceledException("File is in read-only mode");
			}
			if (Length > _stream.Length)
			{
				_stream.SetLength(Length);
			}
			var num = 0L;
			for (var dataBlock = _dataMap.FirstBlock; dataBlock != null; dataBlock = dataBlock.NextBlock)
			{
				if (dataBlock is FileDataBlock fileDataBlock && fileDataBlock.FileOffset != num)
				{
					MoveFileBlock(fileDataBlock, num);
				}
				num += dataBlock.Length;
			}
			num = 0L;
			for (var dataBlock2 = _dataMap.FirstBlock; dataBlock2 != null; dataBlock2 = dataBlock2.NextBlock)
			{
				if (dataBlock2 is MemoryDataBlock memoryDataBlock)
				{
					_stream.Position = num;
					for (var i = 0; i < memoryDataBlock.Length; i += 4096)
					{
						_stream.Write(memoryDataBlock.Data, i, (int)Math.Min(4096L, memoryDataBlock.Length - i));
					}
				}
				num += dataBlock2.Length;
			}
			_stream.SetLength(Length);
			ReInitialize();
		}

		public bool SupportsWriteByte()
		{
			return !_readOnly;
		}

		public bool SupportsInsertBytes()
		{
			return !_readOnly;
		}

		public bool SupportsDeleteBytes()
		{
			return !_readOnly;
		}

		~DynamicFileByteProvider()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_stream != null)
			{
				_stream.Close();
				_stream = null;
			}
			_fileName = null;
			_dataMap = null;
			GC.SuppressFinalize(this);
		}

		private void OnLengthChanged(EventArgs e)
		{
            LengthChanged?.Invoke(this, e);
        }

		private void OnChanged(EventArgs e)
		{
            Changed?.Invoke(this, e);
        }

		private DataBlock GetDataBlock(long findOffset, out long blockOffset)
		{
			if (findOffset < 0 || findOffset > Length)
			{
				throw new ArgumentOutOfRangeException(nameof(findOffset));
			}
			blockOffset = 0L;
			for (var dataBlock = _dataMap.FirstBlock; dataBlock != null; dataBlock = dataBlock.NextBlock)
			{
				if ((blockOffset <= findOffset && blockOffset + dataBlock.Length > findOffset) || dataBlock.NextBlock == null)
				{
					return dataBlock;
				}
				blockOffset += dataBlock.Length;
			}
			return null;
		}

		private FileDataBlock GetNextFileDataBlock(DataBlock block, long dataOffset, out long nextDataOffset)
		{
			nextDataOffset = dataOffset + block.Length;
			for (block = block.NextBlock; block != null; block = block.NextBlock)
			{
				if (block is FileDataBlock result)
				{
					return result;
				}
				nextDataOffset += block.Length;
			}
			return null;
		}

		private byte ReadByteFromFile(long fileOffset)
		{
			if (_stream.Position != fileOffset)
			{
				_stream.Position = fileOffset;
			}
			return (byte)_stream.ReadByte();
		}

		private void MoveFileBlock(FileDataBlock fileBlock, long dataOffset)
		{
			long nextDataOffset;
			var nextFileDataBlock = GetNextFileDataBlock(fileBlock, dataOffset, out nextDataOffset);
			if (nextFileDataBlock != null && dataOffset + fileBlock.Length > nextFileDataBlock.FileOffset)
			{
				MoveFileBlock(nextFileDataBlock, nextDataOffset);
			}
			if (fileBlock.FileOffset > dataOffset)
			{
				var array = new byte[4096];
				for (var num = 0L; num < fileBlock.Length; num += array.Length)
				{
					var position = fileBlock.FileOffset + num;
					var count = (int)Math.Min(array.Length, fileBlock.Length - num);
					_stream.Position = position;
					_stream.Read(array, 0, count);
					var position2 = dataOffset + num;
					_stream.Position = position2;
					_stream.Write(array, 0, count);
				}
			}
			else
			{
				var array2 = new byte[4096];
				for (var num2 = 0L; num2 < fileBlock.Length; num2 += array2.Length)
				{
					var num3 = (int)Math.Min(array2.Length, fileBlock.Length - num2);
					var position3 = fileBlock.FileOffset + fileBlock.Length - num2 - num3;
					_stream.Position = position3;
					_stream.Read(array2, 0, num3);
					var position4 = dataOffset + fileBlock.Length - num2 - num3;
					_stream.Position = position4;
					_stream.Write(array2, 0, num3);
				}
			}
			fileBlock.SetFileOffset(dataOffset);
		}

		private void ReInitialize()
		{
			_dataMap = new DataMap();
			_dataMap.AddFirst(new FileDataBlock(0L, _stream.Length));
			Length = _stream.Length;
		}
	}
}
