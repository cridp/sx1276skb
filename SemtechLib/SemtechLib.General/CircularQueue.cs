using System.Collections.Generic;

namespace SemtechLib.General
{
	public sealed class CircularQueue<T> : Queue<T>
	{
		public int MaxLength { get; set; }

		public CircularQueue(int maxLength)
		{
			MaxLength = maxLength;
		}

		public void Add(T item)
		{
			if (Count < MaxLength)
			{
				Enqueue(item);
				return;
			}
			Dequeue();
			Enqueue(item);
		}
	}
}
