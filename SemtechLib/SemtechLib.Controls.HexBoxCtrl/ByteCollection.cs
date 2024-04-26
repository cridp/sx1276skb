using System.Collections;

namespace SemtechLib.Controls.HexBoxCtrl
{
	public sealed class ByteCollection : CollectionBase
	{
		public byte this[int index]
		{
			get => (byte)List[index];
			set => List[index] = value;
		}

		public ByteCollection()
		{
		}

		public ByteCollection(byte[] bs)
		{
			AddRange(bs);
		}

		public void Add(byte b)
		{
			List.Add(b);
		}

		public void AddRange(byte[] bs)
		{
			InnerList.AddRange(bs);
		}

		public void Remove(byte b)
		{
			List.Remove(b);
		}

		public void RemoveRange(int index, int count)
		{
			InnerList.RemoveRange(index, count);
		}

		public void InsertRange(int index, byte[] bs)
		{
			InnerList.InsertRange(index, bs);
		}

		public byte[] GetBytes()
		{
			var array = new byte[Count];
			InnerList.CopyTo(0, array, 0, array.Length);
			return array;
		}

		public void Insert(int index, byte b)
		{
			InnerList.Insert(index, b);
		}

		public int IndexOf(byte b)
		{
			return InnerList.IndexOf(b);
		}

		public bool Contains(byte b)
		{
			return InnerList.Contains(b);
		}

		public void CopyTo(byte[] bs, int index)
		{
			InnerList.CopyTo(bs, index);
		}

		public byte[] ToArray()
		{
			var array = new byte[Count];
			CopyTo(array, 0);
			return array;
		}
	}
}
